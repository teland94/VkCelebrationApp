using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IUserService
    {
        Task<VkUserDto> GetUserInfoAsync(bool withProfileFields = false);

        Task<int> DetectAgeAsync(long userId, string firstName, string lastName);

        Task CreateAsync(UserDto user);

        Task<UserDto> GetUserAsync(int id);

        Task<UserDto> GetByVkUserIdAsync(long vkUserId);

        Task AddToIngoreListAsync(long vkUserId, int userId);

        Task<VkCollectionDto<VkUserDto>> GetNoBlacklistedUsersAsync(VkCollectionDto<VkUserDto> users, int userId);

        Task DeleteFromIngoreListAsync(int id);
    }
}