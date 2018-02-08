using System;
using VkCelebrationApp.DAL.Configuration;
using VkCelebrationApp.DAL.Entities;
using VkCelebrationApp.DAL.Interfaces;

namespace VkCelebrationApp.DAL.EF
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationContext _db;

        private IGenericRepository<User> _usersRepository;
        private IGenericRepository<UserCongratulation> _userCongratulationsRepository;
        private IGenericRepository<CongratulationTemplate> _congratulationTemplatesRepository;

        public UnitOfWork(IConnectionStringsConfiguration connectionStringsConfiguration)
        {
            _db = new ApplicationContext(connectionStringsConfiguration);
            _db.Database.EnsureCreated();
        }

        public IGenericRepository<User> UsersRepository => 
            _usersRepository ?? (_usersRepository = new EFGenericRepository<User>(_db));


        public IGenericRepository<UserCongratulation> UserCongratulationsRepository => 
            _userCongratulationsRepository ?? (_userCongratulationsRepository = new EFGenericRepository<UserCongratulation>(_db));

        public IGenericRepository<CongratulationTemplate> CongratulationTemplatesRepository => 
            _congratulationTemplatesRepository ?? (_congratulationTemplatesRepository = new EFGenericRepository<CongratulationTemplate>(_db));


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
