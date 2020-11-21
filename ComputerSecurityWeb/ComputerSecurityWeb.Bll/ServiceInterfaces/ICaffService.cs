using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComputerSecurityWeb.Bll.ServiceInterfaces
{
    public interface ICaffService
    {
        public Task<int> TestDll(int i);

    }
}
