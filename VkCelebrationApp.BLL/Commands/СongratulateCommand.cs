using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VkCelebrationApp.BLL.Dtos;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.BLL.Commands
{
    public class СongratulateCommand : Command
    {
        private readonly ICongratulationTemplatesService _congratulationTemplatesService;
        private readonly IVkCelebrationService _vkCelebrationService;
        private readonly IVkCelebrationStateService _vkCelebrationStateService;

        public СongratulateCommand(string botName,
            IVkCelebrationService vkCelebrationService,
            IVkCelebrationStateService vkCelebrationStateService,
            ICongratulationTemplatesService congratulationTemplatesService)
            : base(botName, vkCelebrationService)
        {
            _congratulationTemplatesService = congratulationTemplatesService;
            _vkCelebrationService = vkCelebrationService;
            _vkCelebrationStateService = vkCelebrationStateService;
        }

        public override string Name => "congratulate";

        public override string LocalizedName => "поздравить";

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            await client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var currentState = _vkCelebrationStateService.GetState();
            var currentUser = currentState.CurrentUsers.FirstOrDefault();
            if (currentUser != null)
            {
                var template = await _congratulationTemplatesService.GetRandomCongratulationTemplateAsync();

                if (template != null)
                {
                    await _vkCelebrationService.SendCongratulationAsync(new UserCongratulationDto
                    {
                        VkUserId = currentUser.Id,
                        Text = template.Text
                    });

                    await client.SendTextMessageAsync(message.Chat.Id, template.Text);
                }
            }
            else
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Не выбрано именинника");
            }
        }
    }
}
