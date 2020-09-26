using Microsoft.EntityFrameworkCore;
using Onion.Application.Exceptions;
using Onion.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Onion.Domain.Common;

namespace Onion.Shared.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IAppDbContext _context;

        public Repository(IAppDbContext context)
        {
            _context = context;
        }

        protected DbSet<TEntity> Entities => _context.Set<TEntity>();

        public Task<TEntity> FirstOrDefault(long id) =>
            Entities.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<long> Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Entities.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<long> Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<long> Delete(long id)
        {
            if (id == default)
                throw new ArgumentNullException(nameof(id));

            TEntity entity = await this.FirstOrDefault(id);

            if (entity == null)
                throw new RecordNotFoundException(id, typeof(TEntity));

            Entities.Remove(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<long> Delete(TEntity entity)
        {
            if (entity == null)
                throw new RecordNotFoundException(default, typeof(TEntity));

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