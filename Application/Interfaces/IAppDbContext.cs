using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Product> Products { get; set; }

        Task<int> SaveChangesAsync();

        DbSet<T> Set<T>() where T : class;
    }
}