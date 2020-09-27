using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Onion.Domain.Common;

namespace Onion.Application.Interfaces
{
    public interface IRepository<TEntity, TKey>
        where TEntity : IEntity<TKey>
    {
        Task<TEntity> FirstOrDefault(TKey id);

        Task<TKey> Insert(TEntity entity);

        Task<TKey> Update(TEntity entity);

        Task<TKey> Delete(TKey id);

        Task<TKey> Delete(TEntity entity);

        Task<List<TEntity>> Get(int pageNumber, int pageSize);

        Task<int> CountAsync();
    }

    public interface IRepository<TEntity> : IRepository<TEntity, long>
        where TEntity : BaseEntity
    {
    }
}