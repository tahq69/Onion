using System.Threading;
using System.Threading.Tasks;
using Onion.Domain.Entities;
using Onion.Identity.Models;

namespace Onion.Identity.Interfaces
{
    public interface IUserRepository
    {
        Task AddRefreshToken(string userId, RefreshToken token, CancellationToken ct);

        Task<ApplicationUser?> SingleByRefreshToken(string token, CancellationToken ct);

        Task UpdateRefreshToken(RefreshToken token, CancellationToken ct);
    }
}