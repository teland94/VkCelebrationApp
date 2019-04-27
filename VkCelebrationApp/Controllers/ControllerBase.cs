using Microsoft.AspNetCore.Mvc;

namespace VkCelebrationApp.Controllers
{
    public class ControllerBase : Controller
    {
        protected int GetUserId()
        {
            return int.Parse(User.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id)?.Value);
        }
    }
}
