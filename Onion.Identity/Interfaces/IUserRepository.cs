using System.Threading;
using System.Threading.Tasks;
using Onion.Domain.Entities;
using Onion.Identity.Models;

namespace Onion.Identity.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> SingleByRefreshToken(string token, CancellationToken ct);


        Task<ApplicationUser> SingleByEmail(string userEmail, CancellationToken ct);

        Task AddRefreshToken(string userId, RefreshToken token, CancellationToken ct);

        Task UpdateRefreshToken(RefreshToken token, CancellationToken ct);

        #region Overloads without CancellationToken

        Task<ApplicationUser?> SingleByRefreshToken(string token) =>
            SingleByRefreshToken(token, CancellationToken.None);

        Task<ApplicationUser> SingleByEmail(string userEmail) =>
            SingleByEmail(userEmail, CancellationToken.None);

        Task AddRefreshToken(string userId, RefreshToken token) =>
            AddRefreshToken(userId, token, CancellationToken.None);

        Task UpdateRefreshToken(RefreshToken token) =>
            UpdateRefreshToken(token, CancellationToken.None);

        #endregion
    }
}