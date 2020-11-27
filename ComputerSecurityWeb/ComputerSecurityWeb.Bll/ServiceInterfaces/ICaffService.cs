using ComputerSecurityWeb.Bll.Dtos.Caff;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComputerSecurityWeb.Bll.ServiceInterfaces
{
    public interface ICaffService
    {

        Task<byte[]> GetImageForCaff(Guid caffId);

        Task<List<CaffHeader>> GetAllCaffFiles();

        Task<CaffInfoDto> GetCaffById(Guid id);

        Task AddCaffFile(string name);

        Task AddComment(Guid userId, Guid caffId, string message);

        Task EditCaffFile(Guid caffId, string newName);

        Task DeleteCaffFile(Guid caffId);

    }
}
