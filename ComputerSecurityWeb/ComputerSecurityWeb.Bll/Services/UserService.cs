using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ComputerSecurityWeb.Bll.DTOs.User;
using ComputerSecurityWeb.Bll.Exceptions;
using ComputerSecurityWeb.Bll.ServiceInterfaces;
using ComputerSecurityWeb.Dal;
using ComputerSecurityWeb.Dal.Models;

namespace ComputerSecurityWeb.Bll.Services
{
    public class UserService : IUserService
    {
        private ApplicationDbContext _dbContext;

        private readonly IConfiguration configuration;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public UserService(ApplicationDbContext dbContext,
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager
            )
        {
            _dbContext = dbContext;
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<UserDTO> AuthenticateAsync(string email, string password)
        {
            //checking user email and password
            var result = await this.signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
            {
                var appUser = this.userManager.Users.SingleOrDefault(r => r.Email == email);
                var token =  await GenerateJwtToken(email, appUser);

                return new UserDTO
                {
                    Id = appUser.Id,
                    Email = email,
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    Role = appUser.Role,
                    Token = token
                };
            }
            else
            {
                throw new Exception("Hibás felhasználónév vagy jelszó");
            }
        }

            public async Task<UserDTO> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            if (_dbContext.Users.Any(u => u.Email == email))
            {
                throw new AlreadyExistingException($"A user with the given email: {email} already exists");
            }
            
            //by registration role must be "User"
            var newUser = new AppUser
            {
                Role = Role.User,
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
            };

            var result = await this.userManager.CreateAsync(newUser, password);
            if(result.Succeeded)
            {
                var appUser = this.userManager.Users.SingleOrDefault(r => r.Email == email);
                var token = await GenerateJwtToken(email, appUser);

                return new UserDTO
                {
                    Email = email,
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    Token = token
                };
            }
            else
            {
                throw new Exception("Unexpected error occured");
            }
        }

        public async Task<bool> ChangePassword(string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GenerateJwtToken(string email, AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email,email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(this.configuration["Jwt:ExpiryMinutes"]));

            var token = new JwtSecurityToken(
                this.configuration["Jwt:Issuer"],
                this.configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
