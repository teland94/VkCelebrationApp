using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.Controllers
{
    [Route("api/VkCelebration")]
    public class VkCelebrationController : Controller
    {
        private IVkCelebrationService VkCelebrationService { get; }

        public VkCelebrationController(IVkCelebrationService vkCelebrationService)
        {
            VkCelebrationService = vkCelebrationService;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchAsync(ushort ageFrom, ushort ageTo)
        {
            var users = await VkCelebrationService.SearchAsync(ageFrom, ageTo);

            return Ok(users);
        }

        [HttpGet("DetectAge")]
        public async Task<IActionResult> DetectAge()
        {
            var users = await VkCelebrationService.DetectAgeAsync(70903149, "Андрей", "Телешев", 15, 25);

            return Ok(users);
        }
    }
}