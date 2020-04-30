using System;
using System.Linq;
using System.Threading.Tasks;
using Cart.Domain;
using Cart.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cart.Infrastructure
{
    public class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : Entity<TKey>
    {
        private readonly ShoppingCartContext _context;

        public EfRepository(ShoppingCartContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> GetAll(params string[] includes)
        {
            var dbSet = _context.Set<TEntity>().AsQueryable();
            dbSet = ApplyIncludes(dbSet, includes);
            return dbSet;
        }

        public async Task<TEntity> GetById(TKey id, params string[] includes)
        {
            var dbSet = _context.Set<TEntity>().AsQueryable();
            dbSet = ApplyIncludes(dbSet, includes);
            try
            {
                return await dbSet.SingleAsync(entity => entity.Id.Equals(id));
            }
            catch (InvalidOperationException exception)
            {
                throw new Exception($"Entity with {id.ToString()} not found!", exception);
            }
        }

        public async Task Create(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }

        private IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> dbSet, string[] includes) =>
            includes.Where(include => include != string.Empty)
                .Aggregate(dbSet, (current, include) => current.Include(include));
    }
}