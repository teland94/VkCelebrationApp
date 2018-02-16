using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VkCelebrationApp.DAL.EF;
using VkCelebrationApp.DAL.Entities;
using VkCelebrationApp.DAL.Interfaces;
using System.Linq;

namespace VkCelebrationApp.DAL.Repositories
{
    internal class CongratulationTemplateRepository : EfGenericRepository<CongratulationTemplate>, ICongratulationTemplateRepository
    {
        public CongratulationTemplateRepository(DbContext context)
            : base(context)
        {
        }

        public async Task<CongratulationTemplate> GetRandomCongratulationTemplateAsync()
        {
            var ids = await DbSet.Select(t => t.Id).ToListAsync();

            var random = new Random();
            var id = ids[random.Next(0, ids.Count - 1)];

            return await DbSet.FindAsync(id);
        }
    }
}
