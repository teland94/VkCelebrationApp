using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VkCelebrationApp.Auth;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.Configuration;
using VkCelebrationApp.Helpers;
using VkCelebrationApp.ViewModels;

namespace VkCelebrationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerConfiguration _jwtIssuerConfiguration;
        private readonly IVkCelebrationService _vkCelebrationService;
        private readonly IUserService _userService;

        public AuthController(IJwtFactory jwtFactory,
            JwtIssuerConfiguration jwtIssuerConfiguration,
            IVkCelebrationService vkCelebrationService,
            IUserService userService)
        {
            _jwtFactory = jwtFactory;
            _jwtIssuerConfiguration = jwtIssuerConfiguration;
            _vkCelebrationService = vkCelebrationService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Vkontakte([FromBody]CredentialsViewModel model)
        {
            Tuple<long, string> authRes;
            try
            {
                authRes = await _vkCelebrationService.Auth(model.Login, model.Password);
            }
            catch (Exception ex)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", ex.Message, ModelState));
            }

            try
            {
                await _userService.CreateAsync(new UserDto
                {
                    UserName = model.Login,
                    VkUserId = authRes.Item1,
                    AccessToken = authRes.Item2
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(Errors.AddErrorToModelState("login_failure", ex.Message, ModelState));
            }

            var localUser = await _userService.GetByVkUserIdAsync(authRes.Item1);

            if (localUser == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Failed to create local user account.", ModelState));
            }

            // generate the jwt for the user...
            var jwt = await Tokens.GenerateJwt(_jwtFactory.GenerateClaimsIdentity(localUser.UserName, localUser.Id.ToString()),
              _jwtFactory, localUser.UserName, _jwtIssuerConfiguration, new JsonSerializerSettings { Formatting = Formatting.Indented });

            return new OkObjectResult(jwt);
        }
    }
}