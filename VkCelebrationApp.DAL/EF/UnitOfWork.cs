using System;
using VkCelebrationApp.DAL.Configuration;
using VkCelebrationApp.DAL.Entities;
using VkCelebrationApp.DAL.Interfaces;
using VkCelebrationApp.DAL.Repositories;

namespace VkCelebrationApp.DAL.EF
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationContext _db;

        private IGenericRepository<User> _usersRepository;
        private ICongratulationTemplateRepository _congratulationTemplatesRepository;
        private IUserCongratulationsRepository _userCongratulationsRepository;

        public UnitOfWork(IConnectionStringsConfiguration connectionStringsConfiguration)
        {
            _db = new ApplicationContext(connectionStringsConfiguration);
            _db.Database.EnsureCreated();
        }

        public IGenericRepository<User> UsersRepository => 
            _usersRepository ?? (_usersRepository = new EfGenericRepository<User>(_db));

        public ICongratulationTemplateRepository CongratulationTemplatesRepository => 
            _congratulationTemplatesRepository ?? (_congratulationTemplatesRepository = new CongratulationTemplateRepository(_db));

        public IUserCongratulationsRepository UserCongratulationsRepository => 
            _userCongratulationsRepository ?? (_userCongratulationsRepository = new UserCongratulationsRepository(_db));


        private bool _disposed;

        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                this._disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
