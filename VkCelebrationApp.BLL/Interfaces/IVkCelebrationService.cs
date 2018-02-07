using System.Collections.Generic;
using System.Threading.Tasks;
using VkNet.Model;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IVkCelebrationService
    {
        Task<IEnumerable<User>> SearchAsync(int ageFrom, int ageTo);

        Task<int> DetectAgeAsync(long id, string firstName, string lastName, int ageFrom, int ageTo)
    }
}
