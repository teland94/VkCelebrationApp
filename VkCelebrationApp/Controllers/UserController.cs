using System;
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
    [Route("api/User")]
    public class UserController : ControllerBase
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
                var user = await UserService.GetUserInfoAsync(true);
                var userVm = Mapper.Map<VkUserDto, VkUserViewModel>(user);

                return Ok(userVm);
            }
            catch (Exception ex)
            {
                return NotFound("Vk User Not Found: " + ex.Message);
            }
        }

        [HttpGet("DetectAge")]
        public async Task<IActionResult> DetectAge(long userId, string firstName, string lastName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var age = await UserService.DetectAgeAsync(userId, firstName, lastName);

            return Ok(age);
        }

        [HttpPost("AddToIngoreList")]
        public async Task<IActionResult> AddToIngoreList([FromBody] AddToIngoreListViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await UserService.AddToIngoreListAsync(vm.VkUserId, GetUserId());

            return NoContent();
        }

        [HttpDelete("DeleteFromIgnoreList")]
        public async Task<IActionResult> DeleteFromIgnoreList(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await UserService.DeleteFromIngoreListAsync(id);

            return NoContent();
        }
    }
}