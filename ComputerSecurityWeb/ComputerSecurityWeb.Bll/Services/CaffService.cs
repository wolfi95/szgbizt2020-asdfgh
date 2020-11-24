using ComputerSecurityWeb.Bll.Dtos.Caff;
using ComputerSecurityWeb.Bll.ServiceInterfaces;
using ComputerSecurityWeb.Dal;
using ComputerSecurityWeb.Dal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        public static extern int Test(int i);


        public async Task<List<CaffHeader>> GetAllCaffFiles()
        {
            ////TODO: Statikus file olvasás helyett a dll függvénye ad itt vissza adatot
            byte[] imageArray = File.ReadAllBytes(@"CaffFiles/asd.bmp");
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            var list = new List<CaffHeader>();
            List<CaffFileModel> models = await this.context.CaffFiles
                    .Include(x => x.Comments).ThenInclude(c => c.User)
                .ToListAsync();

            models.ForEach(x =>
            {
                var comments = new List<CommentDto>();
                x.Comments.ForEach(c =>
                {
                    comments.Add(new CommentDto
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        UserName = c.User.UserName,
                        CaffFileId = c.CaffId,
                        Content = c.Content
                    });
                });

                list.Add(new CaffHeader
                {
                    Id = x.Id,
                    Name = x.FileName,
                    ImageData = base64ImageRepresentation,
                    Comments = comments
                });
            });

            return list;
        }

        public async Task<int> TestDll(int i)
        {
            var path = Directory.GetCurrentDirectory();
            return Test(i);
        }


        public async Task<CaffInfoDto> GetCaffById(Guid id)
        {
            var CaffModel = await this.context.CaffFiles.SingleOrDefaultAsync(x => x.Id == id);
            if (CaffModel is null)
            {
                throw new Exception("Caff file with the given ID was not found");
            }

            return  new CaffInfoDto(CaffModel);
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
            var CaffModel = await this.context.CaffFiles.SingleOrDefaultAsync(x => x.Id == caffId);
            if (CaffModel is null)
            {
                throw new Exception("Caff file with the given ID was not found");
            }

            if(message.Length > 0)
            {
                await this.context.Comments.AddAsync(new Comment
                {
                    CaffId = CaffModel.Id,
                    UserId = userId,
                    Content = message
                });
                await this.context.SaveChangesAsync();
            }
        }
    }
}
