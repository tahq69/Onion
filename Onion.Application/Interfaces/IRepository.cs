using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Onion.Domain.Common;

namespace Onion.Application.Interfaces
{
    /// <summary>
    /// Generic entity repository contract.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <typeparam name="TKey">Type of the entity primary key.</typeparam>
    public interface IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// Find first element by primary key or return <c>null</c> if record not found.
        /// </summary>
        /// <param name="id">Record primary key identifier.</param>
        /// <param name="ct">Asynchronous operation cancellation token.</param>
        /// <returns>Sequence first element or <c>null</c> if record not found.</returns>
        Task<TEntity?> FirstOrDefault(TKey id, CancellationToken ct);

        /// <inheritdoc cref="FirstOrDefault(TKey, CancellationToken)"/>
        Task<TEntity?> FirstOrDefault(TKey id)
            => FirstOrDefault(id, CancellationToken.None);

        /// <summary>
        /// Insert <paramref name="entity"/> to the database.
        /// </summary>
        /// <param name="entity">Record to be inserted.</param>
        /// <returns>Inserted record identifier.</returns>
        Task<TKey> Insert(TEntity entity);

        /// <summary>
        /// Update <paramref name="entity"/> to the database.
        /// </summary>
        /// <param name="entity">Record to be updated.</param>
        /// <returns>Updated record identifier.</returns>
        Task<TKey> Update(TEntity entity);

        /// <summary>
        /// Delete record by primary key.
        /// </summary>
        /// <param name="id">Record primary key identifier.</param>
        /// <param name="ct">Asynchronous operation cancellation token.</param>
        /// <returns>Deleted record identifier.</returns>
        Task<TKey> Delete(TKey id, CancellationToken ct);

        /// <inheritdoc cref="Delete(TKey, CancellationToken)"/>
        Task<TKey> Delete(TKey id) =>
            Delete(id, CancellationToken.None);

        /// <summary>
        /// Delete record entity.
        /// </summary>
        /// <param name="entity">Record to be deleted.</param>
        /// <returns>Deleted record identifier.</returns>
        Task<TKey> Delete(TEntity entity);

        /// <summary>
        /// Get paginated records.
        /// </summary>
        /// <param name="pageNumber">Number of the page to receive.</param>
        /// <param name="pageSize">Number of records in recordset.</param>
        /// <returns>Collection of the records.</returns>
        Task<List<TEntity>> Get(int pageNumber, int pageSize);

        /// <summary>
        /// Get count of the records.
        /// </summary>
        /// <returns>All record count.</returns>
        Task<int> CountAsync();
    }
}