using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.BLL.Interfaces;

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

        [HttpGet("GetUserCongratulations")]
        public IActionResult GetUserCongratulations()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userCongratulations = UserCongratulationsService.GetUserCongratulations();
            
            return Ok(userCongratulations);
        }
    }
}