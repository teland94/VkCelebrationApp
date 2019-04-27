using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Microsoft.Extensions.DependencyInjection;
using VkCelebrationApp.BLL.Interfaces;
using System.Threading.Tasks;

namespace VkCelebrationApp.Filters
{
    public class VkAuthAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context,
                                                        ActionExecutionDelegate next)
        {
            var contextUser = context.HttpContext.User;
            if (contextUser == null) throw new ArgumentNullException(nameof(contextUser));

            var vkCelebrationService = context.HttpContext.RequestServices.GetService<IVkCelebrationService>();
            var userService = context.HttpContext.RequestServices.GetService<IUserService>();

            var userId = int.Parse(contextUser.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id).Value);
            var user = await userService.GetUserAsync(userId);

            if (user == null) throw new InvalidOperationException("Can't get user from DB");
            if (string.IsNullOrEmpty(user.AccessToken)) throw new InvalidOperationException("Access token is empty");

            await vkCelebrationService.Auth(user.AccessToken);

            await next();
        }
    }
}
