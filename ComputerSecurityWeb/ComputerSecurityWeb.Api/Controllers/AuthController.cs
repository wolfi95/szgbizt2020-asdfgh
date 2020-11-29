using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ComputerSecurityWeb.Bll.DTOs.User;
using ComputerSecurityWeb.Bll.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using ComputerSecurityWeb.Api.Common;

namespace ComputerSecurityWeb.Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ComputerSecurityControllerBase
    {
        private readonly IUserService userService;

        public AuthController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            return new JsonResult(await this.userService.AuthenticateAsync(dto.Email, dto.Password));
        }
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO dto)
        {
            //TODO ROLE
            return new JsonResult(await this.userService.RegisterAsync(dto.FirstName, dto.LastName, dto.Email, dto.Password));

        }
    }
}