using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.DAL.Entities;

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