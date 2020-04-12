using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VkCelebrationApp.BLL.Configuration;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.DAL.EF;
using VkCelebrationApp.DAL.Entities;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using VkUser = VkNet.Model.User;

namespace VkCelebrationApp.BLL.Services
{
    public class UserService : IUserService
    {
        private VkApi VkApi { get; }
        private ApplicationContext DbContext { get; }
        private IMapper Mapper { get; }

        public UserService(VkApi vkApi,
            ApplicationContext dbContext,
            IMapper mapper)
        {
            VkApi = vkApi;
            DbContext = dbContext;
            Mapper = mapper;
        }

        public async Task<VkUserDto> GetUserInfoAsync(bool withProfileFields = false)
        {
            var usersGet = await VkApi.Users.GetAsync(
                new long[] { },
                withProfileFields
                    ? ProfileFields.Photo100 | ProfileFields.Photo50 | ProfileFields.BirthDate | ProfileFields.City | ProfileFields.Timezone
                    : ProfileFields.Timezone);

            var user = usersGet.FirstOrDefault();
            var currentDate = DateTime.UtcNow.AddHours(user.Timezone ?? 0);

            return Mapper.Map<VkUser, VkUserDto>(user, opts => opts.Items["CurrentDate"] = currentDate);
        }

        public async Task<int> DetectAgeAsync(long userId, string firstName, string lastName)
        {
            var query = firstName + ' ' + lastName;
            ushort counter;

            long cityId = await GetCityId();

            for (counter = 18;
                counter <= 28; counter++)
            {
                if (await UserExistsAsync(userId, query, counter, cityId))
                {
                    return counter;
                }
                await Task.Delay(200);
            }
            return 0;
        }

        public async Task CreateAsync(UserDto userDto)
        {
            var user = Mapper.Map<UserDto, User>(userDto);
            if (!DbContext.Users.Any(u => u.VkUserId == user.VkUserId))
            {
                await DbContext.Users.AddAsync(user);
            }
            else
            {
                var dbUser = DbContext.Users.FirstOrDefault(u => u.VkUserId == userDto.VkUserId);
                if (dbUser.AccessToken != user.AccessToken)
                {
                    dbUser.AccessToken = user.AccessToken;
                    DbContext.Users.Update(dbUser);
                }
            }
            await DbContext.SaveChangesAsync();
        }

        public async Task<UserDto> GetUserAsync(int id)
        {
            return Mapper.Map<User, UserDto>(await DbContext.Users.FindAsync(id));
        }

        public async Task<UserDto> GetByVkUserIdAsync(long vkUserId)
        {
            return Mapper.Map<User, UserDto>(await DbContext.Users.FirstOrDefaultAsync(u => u.VkUserId == vkUserId));
        }

        public async Task AddToIngoreListAsync(long vkUserId, int userId)
        {
            await DbContext.IgnoredUsers.AddAsync(new IgnoredUser
            {
                VkUserId = vkUserId,
                Date = DateTime.UtcNow,
                UserId = userId
            });
            await DbContext.SaveChangesAsync();
        }

        public async Task<VkCollectionDto<VkUserDto>> GetNoBlacklistedUsersAsync(VkCollectionDto<VkUserDto> users, int userId)
        {
            var vkIds = users.Select(u => u.Id);
            var existsIds = await DbContext.IgnoredUsers.Where(iu =>
                vkIds.Any(vid => vid == iu.VkUserId) && iu.UserId == userId)
                .Select(iu => iu.VkUserId)
                .ToListAsync();
            return users.Where(u => existsIds.All(eid => eid != u.Id)).ToVkCollectionDto(users.TotalCount);
        }

        public async Task DeleteFromIngoreListAsync(int id)
        {
            DbContext.IgnoredUsers.Remove(new IgnoredUser
            {
                Id = id
            });
            await DbContext.SaveChangesAsync();
        }

        private async Task<long> GetCityId()
        {
            return (await VkApi.Users.GetAsync(new long[] { }, ProfileFields.City)).FirstOrDefault().City.Id.Value;
        }

        private async Task<bool> UserExistsAsync(long id, string query, ushort ageTo, long cityId)
        {
            var users = await VkApi.Users.SearchAsync(new UserSearchParams
            {
                Query = query,
                AgeTo = ageTo,
                City = (int?)cityId,
            });

            return users.TotalCount > 0 && users.Any(u => u.Id == id);
        }
    }
}
