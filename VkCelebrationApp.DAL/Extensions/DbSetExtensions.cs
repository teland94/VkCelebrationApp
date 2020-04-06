using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VkCelebrationApp.DAL.Entities;

namespace VkCelebrationApp.DAL.Extensions
{
    public static class DbSetExtensions
    {
        public static async Task<TEntity> GetRandomItemAsync<TEntity>(this DbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate = null) 
            where TEntity: EntityBase
        {
            var query = CreateQueryFromPredicate(dbSet, predicate);

            var ids = await query.Select(t => t.Id).ToListAsync();

            if (ids.Count == 0)
            {
                return null;
            }

            var random = new Random();
            var id = ids[random.Next(ids.Count)];

            return await query.FirstOrDefaultAsync(t => t.Id == id);
        }

        public static async Task<IEnumerable<TEntity>> GetRandomItemsAsync<TEntity>(this DbSet<TEntity> dbSet, int count, Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : EntityBase
        {
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count must be greater than 0");
            }

            var query = CreateQueryFromPredicate(dbSet, predicate);

            var ids = await query.Select(t => t.Id).ToListAsync();

            if (ids.Count == 0)
            {
                return new List<TEntity>();
            }

            var random = new Random();
            var randomIds = new List<int>();

            if (ids.Count < count)
            {
                count = ids.Count;
            }

            var i = 0;
            while (i < count)
            {
                var id = ids[random.Next(ids.Count)];
                if (!randomIds.Contains(id))
                {
                    randomIds.Add(id);
                    i++;
                }
            }

            return await query.Where(ct => randomIds.Any(rid => rid == ct.Id)).ToListAsync();
        }

        private static IQueryable<TEntity> CreateQueryFromPredicate<TEntity>(DbSet<TEntity> dbSet, Expression<Func<TEntity, bool>> predicate) where TEntity : EntityBase
        {
            IQueryable<TEntity> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
        }
    }
}
