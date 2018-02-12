﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.Controllers
{
    public class TelegramMessageController : Controller
    {
        private IVkCelebrationTelegramBotService BotService { get; }
        private ILogger<TelegramMessageController> Logger { get; }

        public TelegramMessageController(IVkCelebrationTelegramBotService botService,
            ILogger<TelegramMessageController> logger)
        {
            BotService = botService;
            Logger = logger;
        }

        [Route(@"api/message/update")]
        public async Task<OkResult> Update([FromBody]Update update)
        {
            Logger.LogInformation("Received Bot Message: " + update.Message.Text);

            await BotService.ProcessMessageAsync(update.Message);
            
            return Ok();
        }
    }
}