using System.Linq;
using System.Threading.Tasks;
using Cart.Domain.Shared;

namespace Cart.Domain
{
    public interface IRepository<TEntity, TKey> where TEntity : Entity<TKey>
    {
        IQueryable<TEntity> GetAll(params string[] includes);
        Task<TEntity> GetById(TKey id, params string[] includes);
        Task Create(TEntity entity);
        Task Update(TEntity entity);
    }
}