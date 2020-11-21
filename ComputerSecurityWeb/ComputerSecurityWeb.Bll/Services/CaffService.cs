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
        [DllImport("HelloWorld.dll")]
        public static extern int Double(int i);

        public async Task<int> TestDll(int i)
        {
            var path = Directory.GetCurrentDirectory();
            return Double(i);

        }
    }
}
