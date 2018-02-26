using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.Controllers
{
    public class HomeController : Controller
    {
        IVkCelebrationService VkCelebrationService;

        public HomeController(IVkCelebrationService vkCelebrationService)
        {
            VkCelebrationService = vkCelebrationService;
        }

        public IActionResult Index()
        {
            VkCelebrationService.Auth();

            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
