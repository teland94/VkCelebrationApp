using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.BLL.Helpers;
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
        private IMapper Mapper { get; }

        public UserCongratulationsService(ApplicationContext dbContext,
            VkApi vkApi,
            IMapper mapper)
        {
            DbContext = dbContext;
            VkApi = vkApi;
            Mapper = mapper;
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

            var ids = userCongratulations.Select(uc => uc.VkUserId).ToList();
            var vkUsers = await GetVkUsers(ids);

            foreach (var uc in userCongratulationDtos)
            {
                uc.VkUser = Mapper.Map<VkNet.Model.User, VkUserDto>(vkUsers[uc.VkUserId]);
            }

            return userCongratulationDtos;
        }

        public async Task<byte[]> GetUserCongratulationsExcelDataAsync(int userId, int timezoneOffset, DateTime? congratulationDate = null)
        {
            var userCongrats = await GetUserCongratulationsAsync(userId, congratulationDate, timezoneOffset);

            var userCongratsExcel = new List<ExportUserCongratulationDto>();
            foreach (var uc in userCongrats)
            {
                var congratExcel = new ExportUserCongratulationDto
                {
                    VkUserId = uc.VkUserId,
                    Name = $"{uc.VkUser.FirstName} {uc.VkUser.LastName}",
                    Photo = new Bitmap(await ImageHelpers.DownloadStreamAsync(uc.VkUser.Photo100)),
                    Text = uc.Text,
                    CongratulationDate = uc.CongratulationDate.AddHours(timezoneOffset)
                };
                userCongratsExcel.Add(congratExcel);
            }

            return ExcelHelpers.GetExportFile(userCongratsExcel, "Поздравленные");
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

        private async Task<Dictionary<long, VkNet.Model.User>> GetVkUsers(IList<long> ids)
        {
            const int getUsersLimitCount = 900;
            var iterations = Math.Ceiling(ids.Count / (decimal)getUsersLimitCount);
            var vkUsers = new Dictionary<long, VkNet.Model.User>();
            for (var i = 0; i < iterations; i++)
            {
                var iteratedIds = ids.Skip(getUsersLimitCount * i).Take(getUsersLimitCount);
                var users = (await VkApi.Users.GetAsync(iteratedIds,
                    ProfileFields.Photo100 | ProfileFields.PhotoMaxOrig))
                    .ToDictionary(u => u.Id, u => u);
                vkUsers.AddRange(users);
            }

            return vkUsers;
        }
    }
}
