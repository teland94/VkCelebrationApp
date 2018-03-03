using Microsoft.AspNetCore.Mvc;
using System;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.ViewModels;

namespace VkCelebrationApp.Controllers
{
    [Route("api/UserCongratulations")]
    public class UserCongratulationsController : Controller
    {
        private IUserCongratulationsService UserCongratulationsService { get; }

        public UserCongratulationsController(IUserCongratulationsService userCongratulationService)
        {
            UserCongratulationsService = userCongratulationService;
        }

        [HttpPost("GetUserCongratulations")]
        public IActionResult GetUserCongratulations([FromBody] UserCongratulationsGetParams parameters)
        {
            if (!ModelState.IsValid && parameters != null)
            {
                return BadRequest(ModelState);
            }

            var userCongratulations = UserCongratulationsService
                .GetUserCongratulations(parameters.CongratulationDate, parameters.TimezoneOffset);
            
            return Ok(userCongratulations);
        }
    }
}