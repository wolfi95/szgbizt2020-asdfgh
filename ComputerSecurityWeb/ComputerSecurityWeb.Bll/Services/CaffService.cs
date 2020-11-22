using ComputerSecurityWeb.Bll.Dtos.Caff;
using ComputerSecurityWeb.Bll.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComputerSecurityWeb.Bll.Services
{
    public class CaffService : ICaffService
    {

        public CaffService()
        {
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
    }
}
