using Domain.Common;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> FirstOrDefault(long id);

        Task<long> Insert(T entity);

        Task<long> Update(T entity);

        Task<long> Delete(long id);
    }
}