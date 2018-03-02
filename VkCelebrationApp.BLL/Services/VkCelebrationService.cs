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
    public class VkCelebrationService : IVkCelebrationService, IVkCelebrationStateService
    {
        private readonly IVkApiConfiguration _vkApiConfiguration;
        private readonly IVkSearchConfiguration _vkSearchConfiguration;

        private VkApi VkApi { get; }

        private IUnitOfWork UnitOfWork { get; }
        private IUserService UserService { get; set; }
        private ICongratulationTemplatesService CongratulationTemplatesService { get; }
        private IUserCongratulationsService UserCongratulationService { get; }

        private static VkUserDto _currentUser;

        private static VkCollectionDto<VkUserDto> _currentUsers;
        private static uint _offset;

        public VkCelebrationService(IVkApiConfiguration vkApiConfiguration,
            IUserService userService,
            IVkSearchConfiguration vkSearchConfiguration,
            VkApi vkApi,
            IUnitOfWork unitOfWork,
            ICongratulationTemplatesService congratulationTemplatesService,
            IUserCongratulationsService userCongratulationService)
        {
            _vkApiConfiguration = vkApiConfiguration;
            _vkSearchConfiguration = vkSearchConfiguration;
            VkApi = vkApi;
            UnitOfWork = unitOfWork;
            UserService = userService;
            CongratulationTemplatesService = congratulationTemplatesService;
            UserCongratulationService = userCongratulationService;
        }

        public async Task Auth()
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

            _currentUser = await UserService.GetUserInfoAsync();
        }

        public async Task<VkCollectionDto<VkUserDto>> FindAsync()
        {
            var users = await SearchAsync(1, _offset);

            if (!users.Any()) 
            {
                if (_offset < users.TotalCount)
                {
                    _offset++;
                    await FindAsync();
                }
            }
            else
            {
                _currentUsers = users;
            }

            return _currentUsers;
        }

        public void GoForward()
        {
            _offset++;
            if (_offset >= _currentUsers.TotalCount)
            {
                _offset = 0;
            }
        }

        public VkCelebrationServiceState GetState()
        {
            return new VkCelebrationServiceState
            {
                CurrentUsers = _currentUsers,
                Offset = _offset
            };
        }

        public async Task<VkCollectionDto<VkUserDto>> SearchAsync(uint? count = 1000, uint? offset = 0)
        {
            var date = DateTime.UtcNow.AddHours(_currentUser.TimeZone ?? 0);

            var users = await VkApi.Users.SearchAsync(new UserSearchParams
            {
                Sort = UserSort.ByRegDate,
                AgeFrom = _vkSearchConfiguration.AgeFrom.GetValueOrDefault(),
                AgeTo = _vkSearchConfiguration.AgeTo.GetValueOrDefault(),
                BirthMonth = (ushort)date.Month,
                BirthDay = (ushort)date.Day,
                City = (int?)_currentUser?.CityId,
                Country = (int?)_currentUser?.CountryId,
                Sex = _vkSearchConfiguration.Sex != null && _vkSearchConfiguration.Sex >= 0 && _vkSearchConfiguration.Sex <= 2
                    ? (Sex)_vkSearchConfiguration.Sex.Value : Sex.Unknown,
                Online = true,
                HasPhoto = true,
                Fields = ProfileFields.Photo100 | ProfileFields.PhotoMax | ProfileFields.Photo50 | ProfileFields.CanWritePrivateMessage | ProfileFields.BirthDate 
                         | ProfileFields.Timezone | ProfileFields.City | ProfileFields.Country,
                Count = count,
                Offset = offset
            });

            users = GetCustomFilteredUsers(users);

            var userDtos = Mapper.Map<VkCollection<User>, VkCollectionDto<VkUserDto>>(users);

            userDtos = UserCongratulationService.GetNoCongratulatedUsers(userDtos);

            return userDtos;
        }

        public async Task<int> DetectAgeAsync(long userId, string firstName, string lastName)
        {
            var query = firstName + ' ' + lastName;
            ushort counter;
            for (counter = _vkSearchConfiguration.AgeFrom.GetValueOrDefault(); 
                counter <= _vkSearchConfiguration.AgeTo.GetValueOrDefault(); counter++)
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
            if (!string.IsNullOrWhiteSpace(userCongratulationDto.Text))
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
                    VkUserId = userCongratulationDto.VkUserId,
                    UserId = 1
                });

                await VkApi.Messages.DeleteDialogAsync(userCongratulationDto.VkUserId);

                return messageId;
            }

            throw new ArgumentNullException("userCongratulationDto.Text");
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
                City = (int?)_currentUser?.CityId,
                Country = (int?)_currentUser?.CountryId
            });

            return users.TotalCount > 0 && users.Any(u => u.Id == id);
        }
    }
}
