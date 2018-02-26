using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using VkCelebrationApp.BLL.Extensions;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.BLL.Commands
{
    public abstract class Command
    {
        protected IVkCelebrationService VkCelebrationService { get; }
        protected IVkCelebrationStateService VkCelebrationStateService { get; }

        protected Command(IVkCelebrationService vkCelebrationService, 
            IVkCelebrationStateService vkCelebrationStateService)
        {
            VkCelebrationService = vkCelebrationService;
            VkCelebrationStateService = vkCelebrationStateService;
        }

        public abstract string Name { get; }

        public abstract string LocalizedName { get; }

        public abstract Task Execute(Message message, ITelegramBotClient client);

        public bool Contains(string command)
        {
            return command.Contains(this.Name, StringComparison.OrdinalIgnoreCase)
                || command.Contains(this.LocalizedName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
