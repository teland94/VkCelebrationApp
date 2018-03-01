using System.Collections.Generic;
using VkCelebrationApp.DAL.Entities;

namespace VkCelebrationApp.DAL.Interfaces
{
    public interface IUserCongratulationsRepository : IGenericRepository<UserCongratulation>
    {
        IEnumerable<long> GetExistsVkIds(IEnumerable<long> vkIds);
    }
}
