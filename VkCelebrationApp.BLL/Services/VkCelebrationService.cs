using System;
using System.Collections.Generic;
using VkCelebrationApp.BLL.Interfaces;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using System.Linq;
using VkNet.Model;
using System.Threading.Tasks;
using AutoMapper;
using VkCelebrationApp.BLL.Configuration;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.DAL.Interfaces;
using VkNet.Enums;
using VkNet.Utils;

namespace VkCelebrationApp.BLL.Services
{
    public class VkCelebrationService : IVkCelebrationService
    {
        private readonly IVkApiConfiguration _vkApiConfiguration;

        private VkApi VkApi { get; }

        private IUnitOfWork UnitOfWork { get; }

        public VkCelebrationService(IVkApiConfiguration vkApiConfiguration, 
            VkApi vkApi,
            IUnitOfWork unitOfWork)
        {
            _vkApiConfiguration = vkApiConfiguration;
            VkApi = vkApi;
            UnitOfWork = unitOfWork;
        }

        public void Auth()
        {
            //TODO: Change Auth
            var user = UnitOfWork.UsersRepository.FindById(1);

            VkApi.Authorize(new ApiAuthParams
            {
                ApplicationId = _vkApiConfiguration.AppId,
                Login = user.Login,
                Password = user.Password,
                Settings = Settings.Friends,

                Host = _vkApiConfiguration.Host,
                Port = _vkApiConfiguration.Port
            });
        }

        public async Task<IEnumerable<UserDto>> SearchAsync(ushort ageFrom, ushort ageTo)
        {
            var date = DateTime.Now;

            var users = await VkApi.Users.SearchAsync(new UserSearchParams
            {
                Sort = UserSort.ByRegDate,
                AgeFrom = ageFrom,
                AgeTo = ageTo,
                BirthMonth = (ushort)date.Month,
                BirthDay = (ushort)date.Day,
                City = 280,
                Country = 2,
                Sex = Sex.Female,
                Online = true,
                HasPhoto = true,
                Fields = ProfileFields.Photo100 | ProfileFields.CanWritePrivateMessage | ProfileFields.BirthDate
            });
            
            var totalCount = users.TotalCount;

            var userDtos = Mapper.Map<VkCollection<User>, IEnumerable<UserDto>>(users);
            return userDtos;
        }

        public async Task<int> DetectAgeAsync(long userId, string firstName, string lastName, ushort ageFrom, ushort ageTo)
        {
            var query = firstName + ' ' + lastName;
            ushort counter;
            for (counter = ageFrom; counter < ageTo; counter++)
            {
                if (await UserExistsAsync(userId, query, counter))
                {
                    return counter;
                }
                await Task.Delay(200);
            }
            return counter;
        }

        private async Task<bool> UserExistsAsync(long id, string query, ushort ageTo)
        {
            var users = await VkApi.Users.SearchAsync(new UserSearchParams
            {
                Query = query,
                AgeTo = ageTo,
                City = 280,
                Country = 2
            });

            return users.TotalCount > 0 && users.Any(u => u.Id == id);
        }
    }
}
