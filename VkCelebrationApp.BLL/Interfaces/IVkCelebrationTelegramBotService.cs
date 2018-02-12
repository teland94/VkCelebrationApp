using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IVkCelebrationTelegramBotService
    {
        Task InitClient();

        Task ProcessMessageAsync(Message message);
    }
}