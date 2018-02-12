using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using VkCelebrationApp.BLL.Configuration;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.BLL.Services
{
    public class VkCelebrationTelegramBotService : IVkCelebrationTelegramBotService
    {
        private ITelegramBotClient _client;
        private readonly IBotConfiguration _botConfiguration;
        private readonly IVkCelebrationService _vkCelebrationService;

        private uint _offset;

        public VkCelebrationTelegramBotService(IBotConfiguration botConfiguration,
            IVkCelebrationService vkCelebrationService)
        {
            _botConfiguration = botConfiguration;
            _vkCelebrationService = vkCelebrationService;
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
            var user = users.FirstOrDefault();

            await _client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            if (message.Text.ToLower() == "искать")
            {
                await _client.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(user.PhotoMax.AbsoluteUri));
                await _client.SendTextMessageAsync(message.Chat.Id, GetStrUserInfo(user));

                _offset++;
                if (_offset >= users.TotalCount)
                    _offset = 0;
            }
        }

        private string GetStrUserInfo(UserDto user)
        {
            var sb = new StringBuilder();

            sb.AppendLine(user.FirstName + " " + user.LastName);
            if (user.Age > 0)
            {
                sb.AppendLine("Возраст не определен");
            }

            return sb.ToString();
        }
    }
}
