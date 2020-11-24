using ComputerSecurityWeb.Bll.Dtos.Caff;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComputerSecurityWeb.Bll.ServiceInterfaces
{
    public interface ICaffService
    {
        public Task<int> TestDll(int i);

        public Task<List<CaffHeader>> GetAllCaffFiles();

        public Task<CaffInfoDto> GetCaffById(Guid id);

        public Task AddCaffFile(string name);

        public Task AddComment(Guid caffId, string message);

    }
}
