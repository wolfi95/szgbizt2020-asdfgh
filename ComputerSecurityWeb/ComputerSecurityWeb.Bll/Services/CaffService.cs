using ComputerSecurityWeb.Bll.Dtos.Caff;
using ComputerSecurityWeb.Bll.Dtos.User;
using ComputerSecurityWeb.Bll.ServiceInterfaces;
using ComputerSecurityWeb.Dal;
using ComputerSecurityWeb.Dal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComputerSecurityWeb.Bll.Services
{
    public class CaffService : ICaffService
    {
        private readonly ApplicationDbContext context;

        public CaffService(ApplicationDbContext context)
        {
            this.context = context;
        }

        // Use DllImport to import the Win32 MessageBox function.
        [DllImport("caff_parser.dll")]
        public static unsafe extern void parseCaffToBmpStreamV1(string caffName);

        public async Task<List<CaffHeader>> GetAllCaffFiles()
        {
            var list = new List<CaffHeader>();
            List<CaffFileModel> models = await this.context.CaffFiles
                    .Include(x => x.Comments).ThenInclude(c => c.User)
                .ToListAsync();

            models.ForEach(x =>
            {
                string base64ImageRepresentation = string.Empty;
                byte[] imageArray;
                unsafe
                {
                    string p = $"CaffFiles/{x.FileName}.caff";
                    fixed (char* path = p)
                    {
                        //generates the bmp to send to the client
                        parseCaffToBmpStreamV1(p);
                        imageArray = File.ReadAllBytes(@"preview2.bmp");
                        File.Delete(@"preview2.bmp");
                        base64ImageRepresentation = Convert.ToBase64String(imageArray);
                    }
                }

                list.Add(new CaffHeader
                {
                    Id = x.Id,
                    Name = x.FileName,
                    ImageData = base64ImageRepresentation,
                });
            });

            return list;
        }

        public async Task<CaffInfoDto> GetCaffById(Guid id)
        {
            var caffModel = await this.context.CaffFiles.SingleOrDefaultAsync(x => x.Id == id);
            if (caffModel is null)
            {
                throw new Exception("Caff file with the given ID was not found");
            }

            return new CaffInfoDto(caffModel);
        }

        public async Task AddCaffFile(string name)
        {
            await this.context.CaffFiles.AddAsync(new CaffFileModel
            {
                Id = Guid.NewGuid(),
                FileName = name
            });

            await this.context.SaveChangesAsync();
        }

        public async Task AddComment(Guid userId, Guid caffId, string message)
        {
            var caffModel = await this.context.CaffFiles.SingleOrDefaultAsync(x => x.Id == caffId);
            if (caffModel is null)
            {
                throw new Exception("Caff file with the given ID was not found");
            }

            if (message.Length > 0)
            {
                await this.context.Comments.AddAsync(new Comment
                {
                    CaffId = caffModel.Id,
                    UserId = userId,
                    Content = message
                });
                await this.context.SaveChangesAsync();
            }
        }

        public async Task EditCaffFile(Guid caffId, string newName)
        {
            if (newName.Contains('.') || newName.Contains("\\") || newName.Contains("/"))
            {
                throw new Exception("invalid file name!");
            }

            var caffModel = await this.context.CaffFiles.SingleOrDefaultAsync(x => x.Id == caffId);
            if (caffModel is null)
            {
                throw new Exception("Caff file with the given ID was not found");
            }

            var oldPath = Path.Combine("CaffFiles",
                    $"{caffModel.FileName}.caff");

            var newPath = Path.Combine("CaffFiles",
                    $"{newName}.caff");

            File.Move(oldPath, newPath);
            caffModel.FileName = newName;
            await this.context.SaveChangesAsync();
        }

        public async Task DeleteCaffFile(Guid caffId)
        {
            var caffModel = await this.context.CaffFiles.SingleOrDefaultAsync(x => x.Id == caffId);
            if (caffModel is null)
            {
                throw new Exception("Caff file with the given ID was not found");
            }
            var filePath = Path.Combine("CaffFiles",
                    $"{caffModel.FileName}.caff");

            File.Delete(filePath);
            this.context.CaffFiles.Remove(caffModel);
            await this.context.SaveChangesAsync();
        }

        public async Task<CaffFileDto> GetCaffFileDetailed(Guid id)
        {
            var caffModel = await this.context.CaffFiles
                    .Include(x => x.Comments).ThenInclude(y => y.User)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (caffModel is null)
            {
                throw new Exception("Caff file with the given ID was not found");
            }
            string base64ImageRepresentation = string.Empty;
            byte[] imageArray;
            unsafe
            {
                string p = $"CaffFiles/{caffModel.FileName}.caff";
                fixed (char* path = p)
                {
                    //generates the bmp to send to the client
                    parseCaffToBmpStreamV1(p);
                    imageArray = File.ReadAllBytes(@"preview2.bmp");
                    base64ImageRepresentation = Convert.ToBase64String(imageArray);
                    File.Delete(@"preview2.bmp");
                }
            }
            List<CommentDto> comments = new List<CommentDto>();
            caffModel.Comments.ForEach(x =>
           {
               comments.Add(new CommentDto
               {
                   UserId = x.UserId,
                   Content = x.Content,
                   UserName = x.User.UserName,
               });
           });

            return new CaffFileDto
            {
                Id = caffModel.Id,
                Name = caffModel.FileName,
                ImageData = base64ImageRepresentation,
                Comments = comments
            };
        }

        public async Task<List<EditUserDto>> GetAllUsers()
        {
            var users = await this.context.Users.Where(x => x.Role == Role.User).ToListAsync();
            var userDtoList = new List<EditUserDto>();

            users.ForEach(x =>
            {
                userDtoList.Add(new EditUserDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email
                });
            });

            return userDtoList;
        }

        public async Task EditUserData(EditUserDto dto)
        {
            var user = await this.context.Users.SingleOrDefaultAsync(x => x.Id == dto.Id);
            if(user is null)
            {
                throw new Exception($"User with the given id: {dto.Id} was not found! ");
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;

            await this.context.SaveChangesAsync();
        }

        public async Task<EditUserDto> GetUserData(Guid userId)
        {
            var user = await this.context.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (user is null)
            {
                throw new Exception($"User with the given id: {userId} was not found! ");
            }

            return new EditUserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }
    }
}