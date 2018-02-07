using System;
using System.Collections.Generic;
using VkCelebrationApp.BLL.Interfaces;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using System.Linq;
using VkNet.Model;
using System.Threading.Tasks;

namespace VkCelebrationApp.BLL.Services
{
    public class VkCelebrationService : IVkCelebrationService
    {
        public async Task<IEnumerable<User>> SearchAsync(int ageFrom, int ageTo)
        {
            var date = DateTime.Now;

            var vk = new VkApi();
            vk.Authorize(new ApiAuthParams { Login = "111", Password = "111" });

            var users = await vk.Users.SearchAsync(new UserSearchParams
            {
                Fields = ProfileFields.All,
                Sort = VkNet.Enums.UserSort.ByRegDate,
                AgeFrom = (ushort)ageFrom,
                AgeTo = (ushort)ageTo,
                BirthMonth = (ushort)date.Month,
                BirthDay = (ushort)date.Day,
                City = 280,
                Country = 2,
                Sex = VkNet.Enums.Sex.Female,
                Online = true,
                HasPhoto = true
            });
            var totalCount = users.TotalCount;
            var usersList = users.ToList();

            return users;
        }

        public async Task<int> DetectAgeAsync(long id, string firstName, string lastName, int ageFrom, int ageTo)
        {
            var query = firstName + ' ' + lastName;
            int counter = 0;
            for (counter = ageFrom; counter < ageTo; counter++)
            {
                if (await UserExistsAsync(id, query, counter))
                {
                    return counter;
                }
                await Task.Delay(200);
            }
            return counter;
        }

        private async Task<bool> UserExistsAsync(long id, string query, int ageTo)
        {
            var vk = new VkApi();
            vk.Authorize(new ApiAuthParams { Login = "111", Password = "111" });

            var users = await vk.Users.SearchAsync(new UserSearchParams
            {
                Query = query,
                AgeTo = (ushort)ageTo,
                City = 280,
                Country = 2
            });

            return users.TotalCount > 0 && users.Any(u => u.Id == id);
        }
    }
}
