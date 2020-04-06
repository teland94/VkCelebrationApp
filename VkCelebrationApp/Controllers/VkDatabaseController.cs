using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.Filters;
using VkCelebrationApp.ViewModels;

namespace VkCelebrationApp.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [VkAuth]
    [Route("api/VkDatabase")]
    public class VkDatabaseController : Controller
    {
        private IVkDatabaseService VkDatabaseService { get; }
        private IMapper Mapper { get; }

        public VkDatabaseController(IVkDatabaseService vkDatabaseService,
            IMapper mapper)
        {
            VkDatabaseService = vkDatabaseService;
            Mapper = mapper;
        }

        [HttpGet(nameof(GetCities))]
        public async Task<IActionResult> GetCities(int countryId, string query = "")
        {
            return Ok(Mapper.Map<VkCollectionDto<VkCityDto>, VkCollectionViewModel<VkCityDto>>
                (await VkDatabaseService.GetCitiesAsync(countryId, query)));
        }

        [HttpGet(nameof(GetUniversities))]
        public async Task<IActionResult> GetUniversities(int countryId, int cityId, string query = "")
        {
            return Ok(Mapper.Map<VkCollectionDto<VkUniversityDto>, VkCollectionViewModel<VkUniversityDto>>
                (await VkDatabaseService.GetUniversitiesAsync(countryId, cityId, query)));
        }
    }
}
