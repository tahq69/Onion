using System.Threading.Tasks;
using Domain.Common;

namespace Onion.Application.Interfaces
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> FirstOrDefault(long id);

        Task<long> Insert(TEntity entity);

        Task<long> Update(TEntity entity);

        Task<long> Delete(long id);

        Task<long> Delete(TEntity entity);
    }
}