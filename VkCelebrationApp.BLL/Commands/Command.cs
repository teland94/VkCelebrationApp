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
        private readonly string _botName;

        protected IVkCelebrationService VkCelebrationService { get; }

        protected Command(string botName, IVkCelebrationService vkCelebrationService)
        {
            _botName = botName;
            VkCelebrationService = vkCelebrationService;
        }

        public abstract string Name { get; }

        public abstract Task Execute(Message message, ITelegramBotClient client);

        public bool Contains(string command)
        {
            //return command.Contains(this.Name) && command.Contains(_botName);
            return command.Contains(this.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
