using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserInfoAsync();
    }
}