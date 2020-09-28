using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Onion.Application.Interfaces;
using Onion.Domain.Entities;
using Onion.Identity.Contexts;
using Onion.Identity.Interfaces;
using Onion.Identity.Models;

namespace Onion.Identity.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _dbContext;

        public UserRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApplicationUser> SingleByEmail(string userEmail, CancellationToken ct) =>
            await _dbContext.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == userEmail, ct);


        public async Task AddRefreshToken(string userId, RefreshToken token, CancellationToken ct)
        {
            var user = await _dbContext.Users
                .Include(u => u.RefreshTokens)
                .SingleAsync(u => u.Id.Equals(userId), ct);

            user.RefreshTokens.Add(token);

            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<ApplicationUser> SingleByRefreshToken(string token, CancellationToken ct) =>
            await _dbContext.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token), ct);

        public async Task UpdateRefreshToken(RefreshToken token, CancellationToken ct)
        {
            _dbContext.Update(token);
            await _dbContext.SaveChangesAsync(ct);
        }
    }
}