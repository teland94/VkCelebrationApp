using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VkCelebrationApp.DAL.EF;
using VkCelebrationApp.DAL.Entities;
using VkCelebrationApp.DAL.Interfaces;
using System.Linq;
using System.Collections.Generic;

namespace VkCelebrationApp.DAL.Repositories
{
    public class CongratulationTemplateRepository : EfGenericRepository<CongratulationTemplate>, ICongratulationTemplateRepository
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

        public async Task<IEnumerable<CongratulationTemplate>> GetRandomCongratulationTemplatesAsync(int count)
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count must be greater than 0");
            }

            var ids = await DbSet.Select(t => t.Id).ToListAsync();

            var random = new Random();
            var randomIds = new List<int>();

            var i = 0;
            while (i < count)
            {
                var id = ids[random.Next(0, ids.Count - 1)];
                if (!randomIds.Contains(id))
                {
                    randomIds.Add(id);
                    i++;
                }
            }

            return await DbSet.Where(ct => randomIds.Any(rid => rid == ct.Id)).ToListAsync();
        }
    }
}