using System;
using System.Collections.Generic;
using VkCelebrationApp.BLL.Interfaces;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VkCelebrationApp.BLL.Configuration;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.DAL.Entities;
using VkCelebrationApp.DAL.Interfaces;
using VkNet.Enums;
using VkNet.Utils;
using User = VkNet.Model.User;

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
                Settings = Settings.Friends | Settings.Messages,

                Host = _vkApiConfiguration.Host,
                Port = _vkApiConfiguration.Port
            });
        }

        public async Task<VkCollectionDto<UserDto>> SearchAsync(ushort ageFrom, ushort ageTo, uint? count = 1000, uint? offset = 0)
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
                Fields = ProfileFields.Photo100 | ProfileFields.PhotoMax | ProfileFields.CanWritePrivateMessage | ProfileFields.BirthDate,
                Count = count,
                Offset = offset
            });

            users = GetCustomFilteredUsers(users);
            
            var userDtos = Mapper.Map<VkCollection<User>, VkCollectionDto<UserDto>>(users);

            return userDtos;
        }

        public async Task<int> DetectAgeAsync(long userId, string firstName, string lastName, ushort ageFrom, ushort ageTo)
        {
            var query = firstName + ' ' + lastName;
            ushort counter;
            for (counter = ageFrom; counter <= ageTo; counter++)
            {
                if (await UserExistsAsync(userId, query, counter))
                {
                    return counter;
                }
                await Task.Delay(200);
            }
            return 0;
        }

        public async Task<long> SendCongratulationAsync(UserCongratulationDto userCongratulationDto)
        {
            var messageId = await VkApi.Messages.SendAsync(new MessagesSendParams
            {
                UserId = userCongratulationDto.VkUserId,
                Message = userCongratulationDto.Text
            });

            UnitOfWork.UserCongratulationsRepository.Create(new UserCongratulation
            {
                Text = userCongratulationDto.Text,
                CongratulationDate = DateTime.UtcNow,
                VkUserId = userCongratulationDto.VkUserId
            });

            await VkApi.Messages.DeleteDialogAsync(userCongratulationDto.VkUserId);

            return messageId;
        }

        public IEnumerable<UserCongratulationDto> GetUserCongratulations()
        {
            var userCongratulations = UnitOfWork.UserCongratulationsRepository.Get();

            return Mapper.Map<IEnumerable<UserCongratulation>, IEnumerable<UserCongratulationDto>>(userCongratulations);
        }

        private VkCollection<User> GetCustomFilteredUsers(VkCollection<User> users)
        {
            return users.Where(u => u.CanWritePrivateMessage).ToVkCollection(users.TotalCount);
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
