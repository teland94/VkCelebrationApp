using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IVkCelebrationService
    {
        void Auth();

        Task<VkCollectionDto<UserDto>> SearchAsync(ushort ageFrom, ushort ageTo);

        Task<long> SendCongratulationAsync(UserCongratulationDto userCongratulationDto);

        Task<int> DetectAgeAsync(long userId, string firstName, string lastName, ushort ageFrom, ushort ageTo);
    }
}
