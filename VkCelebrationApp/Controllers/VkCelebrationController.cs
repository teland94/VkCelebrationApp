using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.Filters;
using VkCelebrationApp.Helpers;
using VkCelebrationApp.ViewModels;

namespace VkCelebrationApp.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [VkAuth]
    [Route("api/VkCelebration")]
    public class VkCelebrationController : ControllerBase
    {
        private IVkCelebrationService VkCelebrationService { get; }

        public VkCelebrationController(IVkCelebrationService vkCelebrationService)
        {
            VkCelebrationService = vkCelebrationService;
        }

        [HttpPost("Auth")]
        public async Task<IActionResult> Auth([FromBody]CredentialsViewModel loginViewModel)
        {
            var info = await VkCelebrationService.Auth(loginViewModel.Login, loginViewModel.Password);
            return CreatedAtAction(nameof(Auth), new { VkUserId = info.Item1, AccessToken = info.Item2 });
        }

        [HttpGet("GetFriendsSuggestions")]
        public async Task<IActionResult> GetFriendsSuggestionsAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await VkCelebrationService.GetFriendsSuggestionsAsync(GetUserId());

            return Ok(users);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await VkCelebrationService.SearchAsync(GetUserId());
            var userVms = Mapper.Map<VkCollectionDto<VkUserDto>,
                VkCollectionViewModel<VkUserViewModel>>(users);

            for (int i = 0; i < users.Count; i++)
            {
                userVms[i].Photo100 = await ImageHelpers.Download(users[i].Photo100);
            }

            return Ok(userVms);
        }

        [HttpPost("SendCongratulation")]
        public async Task<IActionResult> SendCongratulationAsync([FromBody] UserCongratulationDto userCongratulation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var messageId = await VkCelebrationService.SendCongratulationAsync(userCongratulation, GetUserId());

            return CreatedAtAction("SendCongratulationAsync", new { id = messageId });
        }

        [HttpGet("SendRandomUserCongratulation")]
        public async Task<IActionResult> SendRandomUserCongratulationAsync()
        {
            try
            {
                var messageId = await VkCelebrationService.SendRandomUserCongratulationAsync(GetUserId());

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
    }
}