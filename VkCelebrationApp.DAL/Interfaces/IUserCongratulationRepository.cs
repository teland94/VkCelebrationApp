using System.Collections.Generic;
using VkCelebrationApp.DAL.Entities;

namespace VkCelebrationApp.DAL.Interfaces
{
    public interface IUserCongratulationRepository : IGenericRepository<UserCongratulation>
    {
        IEnumerable<long> GetExistsVkIds(IEnumerable<long> vkIds);
    }
}
