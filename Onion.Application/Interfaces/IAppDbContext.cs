using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Onion.Domain.Entities;

namespace Onion.Application.Interfaces
{
    /// <summary>
    /// Application database context contract.
    /// </summary>
    public interface IAppDbContext
    {
        /// <summary>
        /// Gets or sets products table reference.
        /// </summary>
        DbSet<Product> Products { get; set; }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous save operation. The task result contains the
        /// number of state entries written to the database.
        /// </returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Creates a <see cref="DbSet{TEntity}" /> that can be used to query and save instances of
        /// <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity for which a set should be returned.</typeparam>
        /// <returns>A set for the given entity type.</returns>
        DbSet<TEntity> Set<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Gets an <see cref="EntityEntry{TEntity}" /> for the given entity. The entry provides
        /// access to change tracking information and operations for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to get the entry for.</param>
        /// <returns>The entry for the given entity.</returns>
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
            where TEntity : class;
    }
}