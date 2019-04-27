using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VkCelebrationApp.BLL.Configuration;
using VkCelebrationApp.BLL.Dtos;
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
        private readonly IVkSearchConfiguration _vkSearchConfiguration;

        private VkApi VkApi { get; }
        private ApplicationContext DbContext { get; }

        public UserService(IVkSearchConfiguration vkSearchConfiguration,
            VkApi vkApi,
            ApplicationContext dbContext)
        {
            _vkSearchConfiguration = vkSearchConfiguration;
            VkApi = vkApi;
            DbContext = dbContext;
        }

        public async Task<VkUserDto> GetUserInfoAsync(bool? withProfileFields = null)
        {
            var usersGet = await VkApi.Users.GetAsync(
                new long[] { },
                withProfileFields != null ?
                    (withProfileFields.Value
                    ? ProfileFields.Photo100 | ProfileFields.Photo50 | ProfileFields.BirthDate
                    : null) 
                : null);

            var user = usersGet.FirstOrDefault();

            return Mapper.Map<VkUser, VkUserDto>(user);
        }

        public async Task<int> DetectAgeAsync(long userId, string firstName, string lastName)
        {
            var query = firstName + ' ' + lastName;
            ushort counter;

            long cityId = await GetCityId();

            for (counter = _vkSearchConfiguration.AgeFrom.GetValueOrDefault();
                counter <= _vkSearchConfiguration.AgeTo.GetValueOrDefault(); counter++)
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

        private async Task<long> GetCityId()
        {
            if (_vkSearchConfiguration.CityId == null)
            {
                return (await VkApi.Users.GetAsync(new long[] { }, ProfileFields.City)).FirstOrDefault().City.Id.Value;
            }
            else
            {
                return _vkSearchConfiguration.CityId.Value;
            }
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
