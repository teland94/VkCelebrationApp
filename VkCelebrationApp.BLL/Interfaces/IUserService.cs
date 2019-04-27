using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IUserService
    {
        Task<VkUserDto> GetUserInfoAsync(bool? withProfileFields = null);

        Task<int> DetectAgeAsync(long userId, string firstName, string lastName);

        Task CreateAsync(UserDto user);

        Task<UserDto> GetUserAsync(int id);

        Task<UserDto> GetByVkUserIdAsync(long vkUserId);
    }
}