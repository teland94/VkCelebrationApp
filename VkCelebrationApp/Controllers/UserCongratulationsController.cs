using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.Filters;
using VkCelebrationApp.Helpers;
using VkCelebrationApp.ViewModels;

namespace VkCelebrationApp.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [VkAuth]
    [Route("api/UserCongratulations")]
    public class UserCongratulationsController : ControllerBase
    {
        private IUserCongratulationsService UserCongratulationsService { get; }

        public UserCongratulationsController(IUserCongratulationsService userCongratulationService)
        {
            UserCongratulationsService = userCongratulationService;
        }

        [HttpPost("GetUserCongratulations")]
        public async Task<IActionResult> GetUserCongratulationsAsync([FromBody] UserCongratulationsGetViewModel parameters)
        {
            if (!ModelState.IsValid && parameters != null)
            {
                return BadRequest(ModelState);
            }

            var userCongratulations = (await UserCongratulationsService
                .GetUserCongratulationsAsync(GetUserId(), parameters.CongratulationDate, parameters.TimezoneOffset)).ToList();
            var userCongratulationVms = Mapper.Map<IEnumerable<UserCongratulationDto>,
                IEnumerable<UserCongratulationViewModel>>(userCongratulations).ToList();

            return Ok(userCongratulationVms);
        }
    }
}