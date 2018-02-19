using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.BLL.Commands
{
    public class DetectAgeCommand : Command
    {
        private readonly IVkCelebrationStateService _vkCelebrationStateService;

        public DetectAgeCommand(string botName, 
            IVkCelebrationService vkCelebrationService, 
            IVkCelebrationStateService vkCelebrationStateService) 
            : base(botName, vkCelebrationService)
        {
            _vkCelebrationStateService = vkCelebrationStateService;
        }

        public override string Name => "detect age";

        public override string LocalizedName => "определить возраст";

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            await client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var currentState = _vkCelebrationStateService.GetState();
            if (currentState.CurrentUsers != null)
            {
                try
                {
                    var currentUser = currentState.CurrentUsers.FirstOrDefault();

                    var age = await VkCelebrationService.DetectAgeAsync(currentUser.Id, currentUser.FirstName,
                        currentUser.LastName);
                    await client.SendTextMessageAsync(message.Chat.Id, "Возраст: " + age);
                }
                catch (Exception ex)
                {
                    await client.SendTextMessageAsync(message.Chat.Id, ex.Message);
                }
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Не выбрано именинника");
            }
        }
    }
}
