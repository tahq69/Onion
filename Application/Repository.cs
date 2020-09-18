using Application.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Application.Exceptions;

namespace Application
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IAppDbContext _context;
        private readonly DbSet<T> _entities;

        public Repository(IAppDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public Task<T> FirstOrDefault(long id) =>
            _entities.FirstOrDefaultAsync(e => e.Id == id);

        public async Task<long> Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entities.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<long> Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<long> Delete(long id)
        {
            if (id == default)
                throw new ArgumentNullException(nameof(id));

            T entity = await this.FirstOrDefault(id);

            if (entity == null)
                throw new RecordNotFoundException(id, typeof(T));

            _entities.Remove(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }
    }
}