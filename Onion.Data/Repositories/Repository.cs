using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Onion.Application.Exceptions;
using Onion.Application.Interfaces;
using Onion.Domain.Common;

namespace Onion.Data.Repositories
{
    /// <summary>
    /// Database entity generic repository implementation.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity record.</typeparam>
    /// <typeparam name="TKey">Type of the entity primary key.</typeparam>
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : struct
    {
        private readonly IAppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="context">Application database context.</param>
        public Repository(IAppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets repository entity recordset.
        /// </summary>
        protected DbSet<TEntity> Entities => _context.Set<TEntity>();

        /// <inheritdoc />
        public async Task<TEntity?> FirstOrDefault(TKey id, CancellationToken ct) =>
            await Entities.FirstOrDefaultAsync(e => e.Id.Equals(id), ct);

        /// <inheritdoc />
        public async Task<TKey> Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        /// <inheritdoc />
        public async Task<TKey> Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        /// <inheritdoc />
        public async Task<TKey> Delete(TKey id, CancellationToken ct)
        {
            if (id.Equals(default(TKey)))
                throw new ArgumentNullException(nameof(id));

            TEntity? entity = await this.FirstOrDefault(id, ct);

            if (entity == null)
                throw new RecordNotFoundException<TKey>(id, typeof(TEntity));

            Entities.Remove(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        /// <inheritdoc />
        public async Task<TKey> Delete(TEntity entity)
        {
            if (entity == null)
                throw new RecordNotFoundException<int>(default, typeof(TEntity));

            Entities.Remove(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        /// <inheritdoc />
        public async Task<List<TEntity>> Get(int pageNumber, int pageSize)
        {
            return await Entities
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc />
        public Task<int> CountAsync() =>
            Entities.CountAsync();
    }
}