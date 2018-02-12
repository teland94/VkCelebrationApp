using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using VkCelebrationApp.BLL.Configuration;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.BLL.Services
{
    public class VkCelebrationTelegramBotService : IVkCelebrationTelegramBotService
    {
        private ITelegramBotClient _client;
        private readonly IBotConfiguration _botConfiguration;
        private readonly IVkCelebrationService _vkCelebrationService;
        private readonly ICongratulationTemplatesService _congratulationTemplatesService;

        private uint _offset;
        private UserDto _currentUser;

        public VkCelebrationTelegramBotService(IBotConfiguration botConfiguration,
            IVkCelebrationService vkCelebrationService,
            ICongratulationTemplatesService congratulationTemplatesService)
        {
            _botConfiguration = botConfiguration;
            _vkCelebrationService = vkCelebrationService;
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

            const int count = 1;
            var users = await _vkCelebrationService.SearchAsync(15, 25, count, _offset);
            _currentUser = users.FirstOrDefault();

            await _client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            if (message.Text.Contains("искать", StringComparison.OrdinalIgnoreCase))
            {
                await _client.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(_currentUser.PhotoMax.AbsoluteUri));
                await _client.SendTextMessageAsync(message.Chat.Id, GetStrUserInfo(_currentUser));

                _offset++;
                if (_offset >= users.TotalCount)
                    _offset = 0;
                _currentUser = null;
            }

            if (message.Text.Contains("поздравить", StringComparison.OrdinalIgnoreCase))
            {
                if (_currentUser != null)
                {
                    var templates = _congratulationTemplatesService.Find("", 1).ToList();

                    if (templates.Any())
                    {
                        await _client.SendTextMessageAsync(message.Chat.Id, templates.FirstOrDefault()?.Text);
                    }
                }
                else
                {
                    await _client.SendTextMessageAsync(message.Chat.Id, "Не выбрано именинника");
                }
            }
        }

        private string GetStrUserInfo(UserDto user)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Номер: " + user.Id);
            sb.AppendLine(user.FirstName + " " + user.LastName);
            if (user.Age > 0)
            {
                sb.AppendLine("Возраст не определен");
            }

            return sb.ToString();
        }
    }
}