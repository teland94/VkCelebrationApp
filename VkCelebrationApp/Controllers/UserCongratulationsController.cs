using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;
using VkCelebrationApp.Filters;
using VkCelebrationApp.Helpers;
using VkCelebrationApp.ViewModels;

namespace VkCelebrationApp.Controllers
{
    [Authorize(Policy = "ApiUser")]
    [VkAuth]
    [Route("api/UserCongratulations")]
    public class UserCongratulationsController : ControllerBase
    {
        private IUserCongratulationsService UserCongratulationsService { get; }
        private IMapper Mapper { get; }

        public UserCongratulationsController(IUserCongratulationsService userCongratulationService,
            IMapper mapper)
        {
            UserCongratulationsService = userCongratulationService;
            Mapper = mapper;
        }

        [HttpPost("GetUserCongratulations")]
        public async Task<IActionResult> GetUserCongratulationsAsync([FromBody] UserCongratulationsGetViewModel parameters)
        {
            if (!ModelState.IsValid && parameters != null)
            {
                return BadRequest(ModelState);
            }

            var userCongratulations = (await UserCongratulationsService
                .GetUserCongratulationsAsync(GetUserId(), parameters.CongratulationDate, parameters.TimezoneOffset)).ToList();
            var userCongratulationVms = Mapper.Map<IEnumerable<UserCongratulationDto>,
                IEnumerable<UserCongratulationViewModel>>(userCongratulations).ToList();

            return Ok(userCongratulationVms);
        }

        [HttpPost(nameof(GetUserCongratulationsExcelData))]
        public async Task<IActionResult> GetUserCongratulationsExcelData([FromBody] ExportUserCongratulationsGetViewModel parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await UserCongratulationsService.GetUserCongratulationsExcelDataAsync(GetUserId(), parameters.TimezoneOffset, parameters.CongratulationDate);
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "User_Congratulations.xlsx");
        }
    }
}