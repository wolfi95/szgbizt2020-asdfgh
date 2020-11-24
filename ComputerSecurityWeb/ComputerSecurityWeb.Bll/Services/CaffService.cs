using ComputerSecurityWeb.Bll.Dtos.Caff;
using ComputerSecurityWeb.Bll.ServiceInterfaces;
using ComputerSecurityWeb.Dal;
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
            //TEST: ///////////////////////////////////////////////////////////

            //TODO: Statikus file olvasás helyett a dll függvénye ad itt vissza adatot
            byte[] imageArray = File.ReadAllBytes(@"CaffFiles/asd.bmp");
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            var list = new List<CaffHeader>();
            list.Add(new CaffHeader
            {
                Id = Guid.NewGuid(),
                Name = "Test1",
                ImageData = base64ImageRepresentation
            });

            return list;


            //TEST OVER: //////////////////////////////////////////////////////
        }

        public async Task<int> TestDll(int i)
        {
            var path = Directory.GetCurrentDirectory();
            return Test(i);

        }

        public async Task Download(Guid caffFileId)
        {

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
    }
}
