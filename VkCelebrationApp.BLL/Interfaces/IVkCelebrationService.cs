using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IVkCelebrationService
    {
        Task Auth();

        Task<VkCollectionDto<VkUserDto>> GetFriendsSuggestionsAsync(uint? count = 500, uint? offset = 0);

        Task<VkCollectionDto<VkUserDto>> SearchAsync(uint? count = 1000, uint? offset = 0);

        Task<long> SendCongratulationAsync(UserCongratulationDto userCongratulationDto);

        Task<int> DetectAgeAsync(long userId, string firstName, string lastName);
    }
}