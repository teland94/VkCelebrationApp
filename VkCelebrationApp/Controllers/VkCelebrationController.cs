using System;
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

        [HttpGet("GetFriendsSuggestions")]
        public async Task<IActionResult> GetFriendsSuggestionsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await VkCelebrationService.GetFriendsSuggestionsAsync();

            return Ok(users);
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
        public async Task<IActionResult> DetectAge(long userId, string firstName, string lastName)
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

        [HttpGet("SendRandomUserCongratulation")]
        public async Task<IActionResult> SendRandomUserCongratulationAsync()
        {
            try
            {
                var messageId = await VkCelebrationService.SendRandomUserCongratulationAsync();

                return Ok(messageId);
            }
            catch(InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUserPhotoes")]
        public async Task<IActionResult> GetUserPhotoes(long userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var photoes = await VkCelebrationService.GetUserPhotoes(userId);

            return Ok(photoes);
        }
    }
}