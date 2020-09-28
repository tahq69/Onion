using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Onion.Application.Exceptions;
using Onion.Application.Interfaces;
using Onion.Domain.Common;

namespace Onion.Data.Repositories
{
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : struct
    {
        private readonly IAppDbContext _context;

        public Repository(IAppDbContext context)
        {
            _context = context;
        }

        protected DbSet<TEntity> Entities => _context.Set<TEntity>();

        public Task<TEntity> FirstOrDefault(TKey id) =>
            Entities.FirstOrDefaultAsync(e => e.Id.Equals(id));

        public async Task<TKey> Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<TKey> Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<TKey> Delete(TKey id)
        {
            if (id.Equals(default(TKey)))
                throw new ArgumentNullException(nameof(id));

            TEntity entity = await this.FirstOrDefault(id);

            if (entity == null)
                throw new RecordNotFoundException<TKey>(id, typeof(TEntity));

            Entities.Remove(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<TKey> Delete(TEntity entity)
        {
            if (entity == null)
                throw new RecordNotFoundException<int>(default, typeof(TEntity));

            Entities.Remove(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<List<TEntity>> Get(int pageNumber, int pageSize)
        {
            return await Entities
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public Task<int> CountAsync() =>
            Entities.CountAsync();
    }
}