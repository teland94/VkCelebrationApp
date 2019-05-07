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
using VkNet.Enums;
using VkNet.Model;
using VkNet.Utils;
using User = VkNet.Model.User;
using VkNet.Exception;
using VkCelebrationApp.DAL.EF;
using LinqKit;

namespace VkCelebrationApp.BLL.Services
{
    public class VkCelebrationService : IVkCelebrationService
    {
        private VkApi VkApi { get; }
        private ApplicationContext DbContext { get; }

        private IUserService UserService { get; set; }
        private ICongratulationTemplatesService CongratulationTemplatesService { get; }
        private IUserCongratulationsService UserCongratulationService { get; }
        private IFaceApiService FaceApiService { get; }

        public VkCelebrationService(IUserService userService,
            VkApi vkApi,
            ApplicationContext dbContext,
            ICongratulationTemplatesService congratulationTemplatesService,
            IUserCongratulationsService userCongratulationService,
            IFaceApiService faceApiService)
        {
            VkApi = vkApi;
            DbContext = dbContext;
            UserService = userService;
            CongratulationTemplatesService = congratulationTemplatesService;
            UserCongratulationService = userCongratulationService;
            FaceApiService = faceApiService;
        }

        public async Task<Tuple<long, string>> Auth(string login, string password)
        {
            try
            {
                await VkApi.AuthorizeAsync(new ApiAuthParams
                {
                    Login = login,
                    Password = password,
                    Settings = Settings.Friends | Settings.Messages | Settings.Photos | Settings.Offline,
                });

                return new Tuple<long, string>(VkApi.UserId.Value, VkApi.Token);
            }
            catch (VkApiAuthorizationException)
            {
                throw new InvalidOperationException("Не удается войти.");
            }
        }

        public async Task Auth(string accessToken)
        {
            await VkApi.AuthorizeAsync(new ApiAuthParams
            {
                AccessToken = accessToken,
            });

            if (VkApi.IsAuthorized) //!!
            {
                var user = await UserService.GetUserInfoAsync();

                await UserService.CreateAsync(new UserDto
                {
                    VkUserId = user.Id,
                    AccessToken = accessToken
                });
            }
            else
            {
                throw new InvalidOperationException("Invalid auth");
            }
        }

        public async Task<VkCollectionDto<VkUserDto>> GetFriendsSuggestionsAsync(int userId, uint? count = 500, uint? offset = 0)
        {
            var currentUser = (await VkApi.Users.GetAsync(new long[] { }, ProfileFields.Timezone)).FirstOrDefault();

            var date = DateTime.UtcNow.AddHours(currentUser.Timezone ?? 0);

            var users = await VkApi.Friends.GetSuggestionsAsync(fields: UsersFields.Photo100 | UsersFields.Photo50 | UsersFields.CanWritePrivateMessage | UsersFields.BirthDate,
                         count: count, offset: offset);

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

            var userDtos = Mapper.Map<VkCollection<VkNet.Model.User>, VkCollectionDto<VkUserDto>>(users);

            userDtos = await UserCongratulationService.GetNoCongratulatedUsersAsync(userDtos, userId);

            return userDtos;
        }

        public async Task<Tuple<VkCollectionDto<VkUserDto>, uint>> SearchAsync(int userId, SearchParamsDto searchParams,
            uint? count = 1000, uint? offset = 0)
        {
            var user = (await VkApi.Users.GetAsync(new long[] { }, ProfileFields.Timezone)).FirstOrDefault();
            var date = DateTime.UtcNow.AddHours(user.Timezone ?? 0);
            var users = await VkApi.CallAsync<VkCollection<User>>("users.search", new UserSearchParams
            {
                Sort = UserSort.ByRegDate,
                AgeFrom = searchParams.AgeFrom,
                AgeTo = searchParams.AgeTo,
                BirthMonth = (ushort)date.Month,
                BirthDay = (ushort)date.Day,
                City = (int?)searchParams.CityId,
                University = (int?)searchParams.UniversityId,
                Sex = (Sex)searchParams.Sex,
                Online = searchParams.Online,
                HasPhoto = true,
                Fields = ProfileFields.Photo100 | ProfileFields.PhotoMax | ProfileFields.Photo50 |
                         ProfileFields.CanWritePrivateMessage | ProfileFields.BirthDate | ProfileFields.Relation,
                Count = 1000,
                //Offset = offset
            });

            var filteredUsers = GetCustomFilteredUsers(users, searchParams);

            var userDtos = Mapper.Map<VkCollection<User>, VkCollectionDto<VkUserDto>>(filteredUsers);

            userDtos = await UserCongratulationService.GetNoCongratulatedUsersAsync(userDtos, userId);

            return new Tuple<VkCollectionDto<VkUserDto>, uint>(
                userDtos.Skip((int)offset).Take((int)count).ToVkCollectionDto(users.TotalCount), 
                (uint)userDtos.Count);
        }

        public async Task<long> SendCongratulationAsync(UserCongratulationDto userCongratulationDto, int userId)
        {
            if (!string.IsNullOrWhiteSpace(userCongratulationDto.Text))
            {
                var messageId = await VkApi.Messages.SendAsync(new MessagesSendParams
                {
                    UserId = userCongratulationDto.VkUserId,
                    Message = userCongratulationDto.Text,
                    RandomId = new Random().Next()
                });

                await DbContext.UserCongratulations.AddAsync(new UserCongratulation
                {
                    Text = userCongratulationDto.Text,
                    CongratulationDate = DateTime.UtcNow,
                    UserId = userId,
                    VkUserId = userCongratulationDto.VkUserId
                });
                await DbContext.SaveChangesAsync();

                return messageId;
            }

            throw new ArgumentNullException("userCongratulationDto.Text");
        }

        public async Task<long> SendRandomUserCongratulationAsync(int userId, SearchParamsDto searchParams)
        {
            var searchUsers = await SearchAsync(userId, searchParams);

            //searchUsers = await GetCustomFilteredUsersByFaces(searchUsers);

            var user = searchUsers.Item1.PickRandom();
            if (user != null)
            {
                var template = await CongratulationTemplatesService.GetRandomCongratulationTemplateAsync(userId);
                if (template != null)
                {
                    return await SendCongratulationAsync(new UserCongratulationDto
                    {
                        VkUserId = user.Id,
                        Text = template.Text
                    }, userId);
                }
                throw new InvalidOperationException("Random Template not found");
            }
            throw new InvalidOperationException("Random User not found");
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

        private VkCollection<User> GetCustomFilteredUsers(VkCollection<User> users, SearchParamsDto searchParam)
        {
            var usersRes = users.AsEnumerable();
            if (searchParam.CanWritePrivateMessage)
            {
                usersRes = usersRes.Where(u => u.CanWritePrivateMessage);
            }
            if (searchParam.IsOpened)
            {
                usersRes = usersRes.Where(u => u.IsClosed != null && !u.IsClosed.Value);
            }
            if (searchParam.RelationTypes != null)
            {
                var rtPredicate = PredicateBuilder.New<User>();
                rtPredicate.Or(u => u.Relation == RelationType.Unknown);
                foreach (var relationType in searchParam.RelationTypes)
                {
                    rtPredicate = rtPredicate.Or(u => u.Relation == (RelationType)relationType);
                }
                usersRes = usersRes.Where(rtPredicate);
            }
            return usersRes.ToVkCollection(users.TotalCount);
        }
    }
}
