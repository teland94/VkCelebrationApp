using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IVkDatabaseService
    {
        Task<VkCollectionDto<VkCityDto>> GetCitiesAsync(int countryId, string query = null);

        Task<VkCollectionDto<VkUniversityDto>> GetUniversitiesAsync(int countryId, int cityId, string query = null);
    }
}