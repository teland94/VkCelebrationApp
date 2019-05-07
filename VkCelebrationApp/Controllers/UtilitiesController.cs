using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.Helpers;

namespace VkCelebrationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UtilitiesController : ControllerBase
    {
        [HttpGet(nameof(GetImageData))]
        public async Task<IActionResult> GetImageData(string url)
        {
            return File(await ImageHelpers.Download(new Uri(url)), "image/jpeg");
        }
    }
}