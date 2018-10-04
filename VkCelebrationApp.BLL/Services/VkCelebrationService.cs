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
using VkCelebrationApp.BLL.Models;
using VkCelebrationApp.DAL.Entities;
using VkCelebrationApp.DAL.Interfaces;
using VkNet.Enums;
using VkNet.Model;
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
        private IFaceApiService FaceApiService { get; }

        private static VkUserDto _currentUser;

        private static VkCollectionDto<VkUserDto> _currentUsers;
        private static uint _offset;

        public VkCelebrationService(IVkApiConfiguration vkApiConfiguration,
            IUserService userService,
            IVkSearchConfiguration vkSearchConfiguration,
            VkApi vkApi,
            IUnitOfWork unitOfWork,
            ICongratulationTemplatesService congratulationTemplatesService,
            IUserCongratulationsService userCongratulationService,
            IFaceApiService faceApiService)
        {
            _vkApiConfiguration = vkApiConfiguration;
            _vkSearchConfiguration = vkSearchConfiguration;
            VkApi = vkApi;
            UnitOfWork = unitOfWork;
            UserService = userService;
            CongratulationTemplatesService = congratulationTemplatesService;
            UserCongratulationService = userCongratulationService;
            FaceApiService = faceApiService;
        }

        public async Task Auth()
        {
            //TODO: Change Auth
            var user = UnitOfWork.UsersRepository.FindById(1);

            VkApi.VkApiVersion.SetVersion(5, 89);

            VkApi.Authorize(new ApiAuthParams
            {
                ApplicationId = _vkApiConfiguration.AppId,
                Login = user.Login,
                Password = user.Password,
                Settings = Settings.Friends | Settings.Messages | Settings.Photos,

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
            if (_offset > _currentUsers.TotalCount)
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

        public async Task<VkCollectionDto<VkUserDto>> GetFriendsSuggestionsAsync(uint? count = 500, uint? offset = 0)
        {
            var date = DateTime.UtcNow.AddHours(_currentUser.TimeZone ?? 0);

            var users = await VkApi.Friends.GetSuggestionsAsync(fields: UsersFields.Photo100 | UsersFields.PhotoMax | UsersFields.Photo50 | UsersFields.CanWritePrivateMessage | UsersFields.BirthDate
                         | UsersFields.Timezone | UsersFields.City | UsersFields.Country, count: count, offset: offset);

            var birthdaySuggestions = new List<User>();
            foreach (var user in users)
            {
                var birthDate = user?.BirthDate.ToDateTime();
                if (birthDate != null && birthDate.Value.Day == date.Day && birthDate.Value.Month == date.Month)
                {
                    birthdaySuggestions.Add(user);
                }
            }
            users = birthdaySuggestions.ToVkCollection((ulong)birthdaySuggestions.Count);

            var userDtos = Mapper.Map<VkCollection<User>, VkCollectionDto<VkUserDto>>(users);

            userDtos = UserCongratulationService.GetNoCongratulatedUsers(userDtos);

            return userDtos;
        }

        public async Task<VkCollectionDto<VkUserDto>> SearchAsync(uint? count = 1000, uint? offset = 0)
        {
            var date = DateTime.UtcNow.AddHours(_currentUser.TimeZone ?? 0);

            var users = await VkApi.CallAsync<VkCollection<UserEx>>("users.search", new UserSearchParams
            {
                Sort = UserSort.ByRegDate,
                AgeFrom = _vkSearchConfiguration.AgeFrom.GetValueOrDefault(),
                AgeTo = _vkSearchConfiguration.AgeTo.GetValueOrDefault(),
                BirthMonth = (ushort) date.Month,
                BirthDay = (ushort) date.Day,
                City = (int?) _currentUser?.CityId,
                Country = (int?) _currentUser?.CountryId,
                Sex = _vkSearchConfiguration.Sex != null && _vkSearchConfiguration.Sex >= 0 &&
                      _vkSearchConfiguration.Sex <= 2
                    ? (Sex) _vkSearchConfiguration.Sex.Value
                    : Sex.Unknown,
                Online = true,
                HasPhoto = true,
                Fields = ProfileFields.Photo100 | ProfileFields.PhotoMax | ProfileFields.Photo50 |
                         ProfileFields.CanWritePrivateMessage | ProfileFields.BirthDate
                         | ProfileFields.Timezone | ProfileFields.City | ProfileFields.Country | ProfileFields.Relation,
                Count = count,
                Offset = offset
            });

            users = GetCustomFilteredUsers(users);

            var userDtos = Mapper.Map<VkCollection<UserEx>, VkCollectionDto<VkUserDto>>(users);

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

                await VkApi.Messages.DeleteConversationAsync(userCongratulationDto.VkUserId);

                return messageId;
            }

            throw new ArgumentNullException("userCongratulationDto.Text");
        }

        public async Task<long> SendRandomUserCongratulationAsync()
        {
            var searchUsers = await SearchAsync();

            searchUsers = await GetCustomFilteredUsersByFaces(searchUsers);

            var user = searchUsers.PickRandom();
            if (user != null)
            {
                var template = await CongratulationTemplatesService.GetRandomCongratulationTemplateAsync();
                return await SendCongratulationAsync(new UserCongratulationDto 
                { 
                    VkUserId = user.Id,
                    Text = template.Text
                });
            }
            throw new InvalidOperationException("User not found");
        }

        public async Task<IEnumerable<string>> GetUserPhotoes(long userId)
        {
            var photoes = await VkApi.Photo.GetAllAsync(new PhotoGetAllParams
            {
                OwnerId = userId,
                Count = 20
            });

            return photoes.Select(p => p?.Photo604.OriginalString);
        }

        private async Task<VkCollectionDto<VkUserDto>> GetCustomFilteredUsersByFaces(VkCollectionDto<VkUserDto> users)
        {
            var vkUsers = new List<VkUserDto>();

            foreach (var user in users)
            {
                var faces = await FaceApiService.DetectAsync(user.PhotoMax);
                if (faces != null)
                {
                    await Task.Delay(4000);
                    if (faces.Any(f => f.FaceAttributes.Gender == "male")
                        && faces.Any(f => f.FaceAttributes.Gender == "female"))
                    {
                        continue;
                    }
                }
                vkUsers.Add(user);
            }

            return vkUsers.ToVkCollectionDto(users.TotalCount);
        }

        private VkCollection<UserEx> GetCustomFilteredUsers(VkCollection<UserEx> users)
        {
            return users.Where(u => u.CanWritePrivateMessage
                                    && !u.IsClosed
                                    && (u.Relation == RelationType.Unknown || u.Relation == RelationType.NotMarried || u.Relation == RelationType.InActiveSearch))
                                    .ToVkCollection(users.TotalCount);
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
