using System.Collections.Generic;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IUserCongratulationsService
    {
        VkCollectionDto<VkUserDto> GetNoCongratulatedUsers(VkCollectionDto<VkUserDto> users);

        IEnumerable<UserCongratulationDto> GetUserCongratulations();
    }
}