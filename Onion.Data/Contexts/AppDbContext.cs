using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;

namespace Onion.Data.Contexts
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("app");
        }
    }
}