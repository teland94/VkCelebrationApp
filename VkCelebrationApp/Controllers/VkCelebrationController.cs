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

        [HttpPost("Search")]
        public async Task<IActionResult> SearchAsync([FromBody] SearchViewModel searchViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await VkCelebrationService.SearchAsync(GetUserId(), 
                Mapper.Map<SearchUserParamsViewModel, SearchParamsDto>(searchViewModel.SearchParams),
                searchViewModel.PageSize, searchViewModel.PageSize * (searchViewModel.PageNumber - 1));
            var userVms = Mapper.Map<VkCollectionDto<VkUserDto>,
                VkCollectionViewModel<VkUserViewModel>>(users.Item1);

            return Ok(new PagedVkCollectionViewModel<VkUserViewModel>
            {
                VkCollection = userVms,
                TotalCount = users.Item2
            });
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

        [HttpPost("SendRandomCongratulation")]
        public async Task<IActionResult> SendRandomUserCongratulationAsync([FromBody] SendRandomCongratulationViewModel sendRandomCongratulationViewModel)
        {
            try
            {
                var messageId = await VkCelebrationService.SendRandomCongratulationAsync(GetUserId(), sendRandomCongratulationViewModel.VkUserId);

                return Ok(messageId);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SendRandomUserCongratulation")]
        public async Task<IActionResult> SendRandomUserCongratulationAsync([FromBody] SearchParamsViewModel searchParams)
        {
            try
            {
                var messageId = await VkCelebrationService.SendRandomUserCongratulationAsync(GetUserId(), 
                    Mapper.Map<SearchParamsViewModel, SearchParamsDto>(searchParams));

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