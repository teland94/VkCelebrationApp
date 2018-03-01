using VkCelebrationApp.DAL.Entities;

namespace VkCelebrationApp.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> UsersRepository { get;  }

        ICongratulationTemplateRepository CongratulationTemplatesRepository { get; }

        IUserCongratulationsRepository UserCongratulationsRepository { get; }
    }
}