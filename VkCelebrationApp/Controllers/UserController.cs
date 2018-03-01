using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.Controllers
{
    [Route("api/User")]
    public class UserController : Controller
    {
        private IUserService UserService { get; }

        public UserController(IUserService userService)
        {
            UserService = userService;
        }

        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfoAsync()
        {
            try
            {
                return Ok(await UserService.GetUserInfoAsync());
            }
            catch (Exception ex)
            {
                return NotFound("Vk User Not Found: " + ex.Message);
            }
        }
    }
}