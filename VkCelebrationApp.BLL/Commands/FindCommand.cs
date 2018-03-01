using System;
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
        public FindCommand(IVkCelebrationService vkCelebrationService,
            IVkCelebrationStateService vkCelebrationStateService) 
            : base(vkCelebrationService, vkCelebrationStateService)
        {
        }

        public override string Name => "find";

        public override string LocalizedName => "искать";

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            await client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            try
            {
                var users = await VkCelebrationStateService.FindAsync();
            
                if (users == null) 
                    return;

                if (users.Any())
                {
                    var user = users.FirstOrDefault();

                    await client.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);
                    await client.SendPhotoAsync(message.Chat.Id, new InputOnlineFile(user.PhotoMaxOrig.AbsoluteUri));

                    await client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                    await client.SendTextMessageAsync(message.Chat.Id, GetStrUserInfo(user));

                    VkCelebrationStateService.GoForward();
                }
                else
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Именинников не найдено");
                }
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Ошибка поиска именинника");
                await client.SendTextMessageAsync(message.Chat.Id, ex.Message);
            }
        }

        private string GetStrUserInfo(VkUserDto user)
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