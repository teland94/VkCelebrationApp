using VkCelebrationApp.DAL.Entities;

namespace VkCelebrationApp.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> UsersRepository { get;  }

        IUserCongratulationRepository UserCongratulationsRepository { get; }

        ICongratulationTemplateRepository CongratulationTemplatesRepository { get; }
    }
}