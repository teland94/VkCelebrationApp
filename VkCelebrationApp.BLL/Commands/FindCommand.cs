using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.BLL.Commands
{
    public class FindCommand : Command
    {
        private IVkCelebrationStateService VkCelebrationStateService { get; }

        public FindCommand(string botName,
            IVkCelebrationService vkCelebrationService,
            IVkCelebrationStateService vkCelebrationStateService) : base(botName, vkCelebrationService)
        {
            VkCelebrationStateService = vkCelebrationStateService;
        }

        public override string Name => "искать";

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            await client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var users = await VkCelebrationStateService.FindAsync(15, 25);
            if (users.Any())
            {
                var user = users.FirstOrDefault();

                await client.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
                await client.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(user.PhotoMax.AbsoluteUri));

                await client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await client.SendTextMessageAsync(message.Chat.Id, GetStrUserInfo(user));

                VkCelebrationStateService.Next();
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Именинников не найдено");
            }
        }

        private string GetStrUserInfo(UserDto user)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("http://vk.com/id" + user.Id);
            sb.AppendLine(user.FirstName + " " + user.LastName);

            if (user.Age > 0)
            {
                sb.AppendLine("Возраст: " + user.Age);
            }
            else
            {
                sb.AppendLine("Возраст не определен");
            }

            return sb.ToString();
        }
    }
}
