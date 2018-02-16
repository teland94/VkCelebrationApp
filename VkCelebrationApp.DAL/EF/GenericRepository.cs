using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VkCelebrationApp.DAL.Interfaces;

namespace VkCelebrationApp.DAL.EF
{
    public class EfGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected DbContext Context;
        protected DbSet<TEntity> DbSet;

        public EfGenericRepository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Get()
        {
            return DbSet.AsNoTracking().ToList();
        }

        public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return DbSet.AsNoTracking().Where(predicate).ToList();
        }

        public IEnumerable<TEntity> Get(int page, int pageSize, Func<TEntity, bool> predicate = null)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            if (predicate != null)
            {
                query = query.Where(predicate).AsQueryable();
            }

            return query.ToList();
        }

        public IEnumerable<TEntity> Get<TKey>(Func<TEntity, TKey> orderBy, bool isAsc = true, Func<TEntity, bool> predicate = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (predicate != null)
            {
                query = query.Where(predicate).AsQueryable();
            }
            query = isAsc ? query.OrderBy(orderBy).AsQueryable() : query.OrderByDescending(orderBy).AsQueryable();

            return query.AsNoTracking().ToList();
        }

        public TEntity FindById(int id)
        {
            return DbSet.Find(id);
        }

        public void Create(TEntity item)
        {
            DbSet.Add(item);
            Context.SaveChanges();
        }

        public void Update(TEntity item)
        {
            Context.Entry(item).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public void Remove(int id)
        {
            var item = DbSet.Find(id);
            if (item == null)
            {
                throw new InvalidOperationException("Item not found");
            }
            DbSet.Remove(item);
            Context.SaveChanges();
        }

        public void Remove(TEntity item)
        {
            DbSet.Remove(item);
            Context.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
