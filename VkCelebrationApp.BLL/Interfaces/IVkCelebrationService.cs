using System.Collections.Generic;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IVkCelebrationService
    {
        void Auth();

        Task<IEnumerable<UserDto>> SearchAsync(ushort ageFrom, ushort ageTo);

        Task<int> DetectAgeAsync(long userId, string firstName, string lastName, ushort ageFrom, ushort ageTo);
    }
}
