using VkCelebrationApp.DAL.Entities;

namespace VkCelebrationApp.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> UsersRepository { get;  }

        IGenericRepository<UserCongratulation> UserCongratulationsRepository { get; }

        ICongratulationTemplateRepository CongratulationTemplatesRepository { get; }
    }
}