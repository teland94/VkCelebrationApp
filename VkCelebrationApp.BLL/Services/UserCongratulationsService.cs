using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.DAL.EF;
using VkCelebrationApp.DAL.Entities;
using VkNet;
using VkNet.Enums.Filters;

namespace VkCelebrationApp.BLL.Services
{
    public class UserCongratulationsService : IUserCongratulationsService
    {
        private ApplicationContext DbContext { get; }
        private VkApi VkApi { get; }

        public UserCongratulationsService(ApplicationContext dbContext,
            VkApi vkApi)
        {
            DbContext = dbContext;
            VkApi = vkApi;
        }

        public async Task<VkCollectionDto<VkUserDto>> GetNoCongratulatedUsersAsync(VkCollectionDto<VkUserDto> users, int userId)
        {
            var vkIds = users.Select(u => u.Id);
            var existsIds = await DbContext.UserCongratulations.Where(uc => 
                vkIds.Any(vid => vid == uc.VkUserId) && uc.UserId == userId)
                .Select(uc => uc.VkUserId)
                .ToListAsync();
            return users.Where(u => existsIds.All(eid => eid != u.Id)).ToVkCollectionDto(users.TotalCount);
        }

        public async Task<IEnumerable<UserCongratulationDto>> GetUserCongratulationsAsync(int userId, DateTime? congratulationDate = null, int? timezoneOffset = null)
        {
            var userCongratulations = await GetUserCongratulationsFiltered(userId, congratulationDate, timezoneOffset);

            var userCongratulationDtos = Mapper.Map<IEnumerable<UserCongratulation>, IEnumerable<UserCongratulationDto>>(userCongratulations);

            var ids = userCongratulations.Select(uc => uc.VkUserId);
            var vkUsers = VkApi.Users.Get(ids,
                ProfileFields.Photo100 | ProfileFields.PhotoMaxOrig)
                .ToDictionary(u => u.Id, u => u);

            foreach(var uc in userCongratulationDtos)
            {
                uc.VkUser = Mapper.Map<VkNet.Model.User, VkUserDto>(vkUsers[uc.VkUserId]);
            }

            return userCongratulationDtos;
        }

        private async Task<IEnumerable<UserCongratulation>> GetUserCongratulationsFiltered(int userId, DateTime? congratulationDate, int? timezoneOffset)
        {
            var userCongratulations = DbContext.UserCongratulations.Where(uc => uc.UserId == userId);

            if (congratulationDate != null)
            {
                userCongratulations = userCongratulations.Where(uc => uc.CongratulationDate.AddHours((double)timezoneOffset).Date == congratulationDate.Value.AddHours((double)timezoneOffset).Date);
            }

            userCongratulations = userCongratulations.OrderByDescending(uc => uc.CongratulationDate);

            return await userCongratulations.ToListAsync();
        }
    }
}
