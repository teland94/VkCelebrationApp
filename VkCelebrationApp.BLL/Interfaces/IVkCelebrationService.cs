using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IVkCelebrationService
    {
        Task<Tuple<long, string>> Auth(string login, string password);

        Task Auth(string accessToken);

        Task<VkCollectionDto<VkUserDto>> GetFriendsSuggestionsAsync(int userId, uint? count = 500, uint? offset = 0);

        Task<Tuple<VkCollectionDto<VkUserDto>, uint>> SearchAsync(int userId, SearchParamsDto searchParams, uint? count = 1000, uint? offset = 0);

        Task<long> SendCongratulationAsync(UserCongratulationDto userCongratulationDto, int userId);

        Task<long> SendRandomUserCongratulationAsync(int userId, SearchParamsDto searchParams);
    }
}