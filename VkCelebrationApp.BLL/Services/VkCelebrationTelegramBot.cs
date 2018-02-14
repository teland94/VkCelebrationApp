using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using VkCelebrationApp.BLL.Commands;
using VkCelebrationApp.BLL.Configuration;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.BLL.Services
{
    public class VkCelebrationTelegramBotService : IVkCelebrationTelegramBotService
    {
        private ITelegramBotClient _client;
        private readonly IBotConfiguration _botConfiguration;
        private readonly IVkCelebrationService _vkCelebrationService;
        private readonly IVkCelebrationStateService _vkCelebrationStateService;
        private readonly ICongratulationTemplatesService _congratulationTemplatesService;

        public VkCelebrationTelegramBotService(IBotConfiguration botConfiguration,
            IVkCelebrationService vkCelebrationService,
            IVkCelebrationStateService vkCelebrationStateService,
            ICongratulationTemplatesService congratulationTemplatesService)
        {
            _botConfiguration = botConfiguration;
            _vkCelebrationService = vkCelebrationService;
            _vkCelebrationStateService = vkCelebrationStateService;
            _congratulationTemplatesService = congratulationTemplatesService;
        }

        public async Task InitClient()
        {
            if (_client != null)
            {
                return;
            }

            _client = new TelegramBotClient(_botConfiguration.Key);
            var hook = string.Concat(_botConfiguration.Url, _botConfiguration.UpdateBaseApiPath);
            await _client.SetWebhookAsync(hook);
        }

        public async Task ProcessMessageAsync(Message message)
        {
            await InitClient();

            var commands = new List<Command>
            {
                new FindCommand("", _vkCelebrationService, _vkCelebrationStateService),
                new СongratulateCommand("", _vkCelebrationService, _vkCelebrationStateService, _congratulationTemplatesService)
            };

            foreach (var command in commands)
            {
                if (command.Contains(message.Text))
                {
                    await command.Execute(message, _client);
                    break;
                }
            }
        }
    }
}