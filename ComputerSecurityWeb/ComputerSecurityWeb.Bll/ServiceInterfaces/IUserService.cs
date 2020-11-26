using System.Threading.Tasks;
using ComputerSecurityWeb.Bll.DTOs.User;

namespace ComputerSecurityWeb.Bll.ServiceInterfaces
{
    public interface IUserService
    {
        Task<UserDTO> AuthenticateAsync(string email, string password);

        Task<UserDTO> RegisterAsync(string firstName, string lastName, string email, string password);

        Task<bool> ChangePassword(string oldPassword, string newPassword);
    }
}
