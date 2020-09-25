using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Onion.Domain.Entities;

namespace Onion.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Product> Products { get; set; }

        Task<int> SaveChangesAsync();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        EntityEntry<TEntity> Entry<TEntity>([NotNull] TEntity entity)
            where TEntity : class;
    }
}