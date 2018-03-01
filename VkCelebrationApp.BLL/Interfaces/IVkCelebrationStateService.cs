using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IVkCelebrationStateService
    {
        Task<VkCollectionDto<VkUserDto>> FindAsync();

        void GoForward();

        VkCelebrationServiceState GetState();
    }
}