using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.BLL.Dtos;
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
        public async Task<IActionResult> SearchAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await VkCelebrationService.SearchAsync();

            return Ok(users);
        }

        [HttpGet("DetectAge")]
        public async Task<IActionResult> DetectAge(long userId, string firstName, string lastName, ushort ageFrom, ushort ageTo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var age = await VkCelebrationService.DetectAgeAsync(userId, firstName, lastName);

            return Ok(age);
        }

        [HttpPost("SendCongratulation")]
        public async Task<IActionResult> SendCongratulationAsync([FromBody] UserCongratulationDto userCongratulation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var messageId = await VkCelebrationService.SendCongratulationAsync(userCongratulation);

            return CreatedAtAction("SendCongratulationAsync", new { id = messageId });
        }

        [HttpGet("GetUserCongratulations")]
        public IActionResult GetUserCongratulations()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var congratulations = VkCelebrationService.GetUserCongratulations();

            return Ok(congratulations);
        }
    }
}