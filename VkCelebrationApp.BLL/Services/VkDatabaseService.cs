using AutoMapper;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams.Database;
using VkNet.Utils;

namespace VkCelebrationApp.BLL.Services
{
    public class VkDatabaseService : IVkDatabaseService
    {
        private VkApi VkApi { get; }
        private IMapper Mapper { get; }

        public VkDatabaseService(VkApi vkApi,
            IMapper mapper)
        {
            VkApi = vkApi;
            Mapper = mapper;
        }

        public async Task<VkCollectionDto<VkCityDto>> GetCitiesAsync(int countryId, string query = "")
        {
            var cities = await VkApi.Database.GetCitiesAsync(new GetCitiesParams
            {
                CountryId = countryId,
                Query = query
            });
            return Mapper.Map<VkCollection<City>, VkCollectionDto<VkCityDto>>(cities);
        }

        public async Task<VkCollectionDto<VkUniversityDto>> GetUniversitiesAsync(int countryId, int cityId, string query = null)
        {
            return Mapper.Map<VkCollection<University>, VkCollectionDto<VkUniversityDto>>
                (await VkApi.Database.GetUniversitiesAsync(countryId, cityId, query));
        }
    }
}
