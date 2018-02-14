using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IVkCelebrationStateService
    {
        Task<VkCollectionDto<UserDto>> FindAsync(ushort ageFrom, ushort ageTo);

        void Next();

        VkCelebrationServiceState GetState();
    }
}