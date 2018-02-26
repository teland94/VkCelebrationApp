using System;
using System.Linq;
using System.Text.RegularExpressions;
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

        public СongratulateCommand(IVkCelebrationService vkCelebrationService,
            IVkCelebrationStateService vkCelebrationStateService,
            ICongratulationTemplatesService congratulationTemplatesService)
            : base(vkCelebrationService, vkCelebrationStateService)
        {
            _congratulationTemplatesService = congratulationTemplatesService;
        }

        public override string Name => "congratulate";

        public override string LocalizedName => "поздравить";

        public override async Task Execute(Message message, ITelegramBotClient client)
        {
            await client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            
            try
            {
                var currentState = VkCelebrationStateService.GetState();
                if (currentState.CurrentUsers != null)
                {
                    var currentUser = currentState.CurrentUsers.FirstOrDefault();

                    var text = GetGongratulationText(message.Text);
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        var template = await _congratulationTemplatesService.GetRandomCongratulationTemplateAsync();

                        if (template != null)
                        {
                            await VkCelebrationService.SendCongratulationAsync(new UserCongratulationDto
                            {
                                VkUserId = currentUser.Id,
                                Text = template.Text
                            });
                            await client.SendTextMessageAsync(message.Chat.Id, template.Text);
                        }
                    }
                    else
                    {
                        await VkCelebrationService.SendCongratulationAsync(new UserCongratulationDto
                        {
                            VkUserId = currentUser.Id,
                            Text = text
                        });
                        await client.SendTextMessageAsync(message.Chat.Id, "Поздравление успешно отправлено");
                        await client.SendTextMessageAsync(message.Chat.Id, text); //Debug!
                    }
                }
                else
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Не выбрано именинника");
                }
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Ошибка при отправке поздравления");
                await client.SendTextMessageAsync(message.Chat.Id, ex.Message); // Debug!
            }
        }

        private string GetGongratulationText(string message)
        {
            var pattern = $@"^(/?{Name}|{LocalizedName})\s+";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!regex.IsMatch(message))
                return null;
            var result = regex.Replace(message, "", 1);
            return result;
        }
    }
}
