using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IUserCongratulationsService
    {
        Task<VkCollectionDto<VkUserDto>> GetNoCongratulatedUsersAsync(VkCollectionDto<VkUserDto> users, int userId);

        Task<IEnumerable<UserCongratulationDto>> GetUserCongratulationsAsync(int userId, DateTime? congratulationDate = null, int? timezoneOffset = null);

        Task<byte[]> GetUserCongratulationsExcelDataAsync(int userId, int timezoneOffset, DateTime? congratulationDate = null);
    }
}