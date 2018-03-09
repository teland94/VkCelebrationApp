using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VkCelebrationApp.DAL.Configuration;
using VkCelebrationApp.DAL.Entities;

namespace VkCelebrationApp.DAL.EF
{
    public class ApplicationContext : IdentityDbContext<AppUser>
    {
        private readonly IConnectionStringsConfiguration _connectionConfiguration;

        public DbSet<User> VkUsers { get; set; }
        public DbSet<UserCongratulation> UserCongratulations { get; set; }
        public DbSet<CongratulationTemplate> CongratulationTemplates { get; set; }

        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }

        public ApplicationContext(IConnectionStringsConfiguration connectionConfiguration)
        {
            _connectionConfiguration = connectionConfiguration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionConfiguration != null)
                optionsBuilder.UseSqlServer(_connectionConfiguration.DefaultConnection);
        }
    }
}
