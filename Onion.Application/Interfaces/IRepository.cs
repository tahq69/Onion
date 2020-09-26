using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Onion.Domain.Common;

namespace Onion.Application.Interfaces
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> FirstOrDefault(long id);

        Task<long> Insert(TEntity entity);

        Task<long> Update(TEntity entity);

        Task<long> Delete(long id);

        Task<long> Delete(TEntity entity);

        Task<List<TEntity>> Get(int pageNumber, int pageSize);

        Task<int> CountAsync();
    }
}