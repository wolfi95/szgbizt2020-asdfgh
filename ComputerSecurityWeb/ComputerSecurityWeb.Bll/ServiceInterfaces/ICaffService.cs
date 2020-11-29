using ComputerSecurityWeb.Bll.Dtos.Caff;
using ComputerSecurityWeb.Bll.Dtos.User;
using ComputerSecurityWeb.Bll.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComputerSecurityWeb.Bll.ServiceInterfaces
{
    public interface ICaffService
    {
        Task<List<CaffHeader>> GetAllCaffFiles();

        Task<CaffInfoDto> GetCaffById(Guid id);

        Task AddCaffFile(string name);

        Task AddComment(Guid userId, Guid caffId, string message);

        Task EditCaffFile(Guid caffId, string newName);

        Task DeleteCaffFile(Guid caffId);

        Task<CaffFileDto> GetCaffFileDetailed(Guid id);

        Task<List<EditUserDto>> GetAllUsers();

        Task<UserDTO> EditUserData(EditUserDto dto);

        Task<EditUserDto> GetUserData(Guid userId);

    }
}
