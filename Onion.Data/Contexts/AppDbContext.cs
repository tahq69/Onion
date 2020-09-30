using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;

namespace Onion.Data.Contexts
{
    /// <summary>
    /// Application database context implementation.
    /// </summary>
    public class AppDbContext : DbContext, IAppDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <inheritdoc />
        public DbSet<Product> Products { get; set; } = null!;

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("app");
        }
    }
}