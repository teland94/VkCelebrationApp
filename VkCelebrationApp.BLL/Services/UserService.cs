using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace VkCelebrationApp.BLL.Services
{
    public class UserService : IUserService
    {
        private VkApi VkApi { get; }

        public UserService(VkApi vkApi)
        {
            VkApi = vkApi;
        }

        public async Task<VkUserDto> GetUserInfoAsync()
        {
            if (VkApi.UserId != null)
            {
                var usersGet = await VkApi.Users.GetAsync(
                    new List<long>
                    {
                        VkApi.UserId.Value
                    },
                    ProfileFields.Photo100 | ProfileFields.PhotoMax | ProfileFields.Photo50 | ProfileFields.BirthDate
                    | ProfileFields.Country | ProfileFields.City | ProfileFields.Timezone
                );
                var user = usersGet.FirstOrDefault();

                return Mapper.Map<User, VkUserDto>(user);
            }

            throw new InvalidOperationException("Invalid Vk Authorization");
        }
    }
}
