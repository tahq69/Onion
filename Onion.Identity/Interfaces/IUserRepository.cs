using System.Threading;
using System.Threading.Tasks;
using Onion.Domain.Entities;
using Onion.Identity.Models;

namespace Onion.Identity.Interfaces
{
    /// <summary>
    /// User record repository contract.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Find application user by refresh token value.
        /// </summary>
        /// <param name="token">Refresh token value.</param>
        /// <param name="ct">Asynchronous operation cancellation token.</param>
        /// <returns><see cref="ApplicationUser"/> entity or <c>null</c> if not found.</returns>
        Task<ApplicationUser?> SingleByRefreshToken(string token, CancellationToken ct);

        /// <summary>
        /// Find application user by email address.
        /// </summary>
        /// <param name="userEmail">User account email address.</param>
        /// <param name="ct">Asynchronous operation cancellation token.</param>
        /// <returns><see cref="ApplicationUser"/> entity or <c>null</c> if not found.</returns>
        Task<ApplicationUser> SingleByEmail(string userEmail, CancellationToken ct);

        /// <summary>
        /// Add refresh token for user.
        /// </summary>
        /// <param name="userId">Application user identifier.</param>
        /// <param name="token">Refresh token.</param>
        /// <param name="ct">Asynchronous operation cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddRefreshToken(string userId, RefreshToken token, CancellationToken ct);

        /// <summary>
        /// Update existing refresh token.
        /// </summary>
        /// <param name="token">Modified refresh token.</param>
        /// <param name="ct">Asynchronous operation cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UpdateRefreshToken(RefreshToken token, CancellationToken ct);

        /// <summary>
        /// Determine whenever user exists in database by identifier value.
        /// </summary>
        /// <param name="userId">Application user identifier.</param>
        /// <param name="ct">Asynchronous operation cancellation token.</param>
        /// <returns><c>true</c> if <param name="userId"/> exists in database, otherwise <c>false</c>.</returns>
        Task<bool> ExistsById(string userId, CancellationToken ct);

        /// <summary>
        /// Determine whenever user exists in database by email address.
        /// </summary>
        /// <param name="userEmail">User account email address.</param>
        /// <param name="ct">Asynchronous operation cancellation token.</param>
        /// <returns><c>true</c> if <param name="userEmail"/> exists in database, otherwise <c>false</c>.</returns>
        Task<bool> ExistsByEmail(string userEmail, CancellationToken ct);

        #region Overloads without CancellationToken

        /// <inheritdoc cref="SingleByRefreshToken(string, CancellationToken)"/>
        Task<ApplicationUser?> SingleByRefreshToken(string token) =>
            SingleByRefreshToken(token, CancellationToken.None);

        /// <inheritdoc cref="SingleByEmail(string, CancellationToken)"/>
        Task<ApplicationUser> SingleByEmail(string userEmail) =>
            SingleByEmail(userEmail, CancellationToken.None);

        /// <inheritdoc cref="AddRefreshToken(string, RefreshToken, CancellationToken)"/>
        Task AddRefreshToken(string userId, RefreshToken token) =>
            AddRefreshToken(userId, token, CancellationToken.None);

        /// <inheritdoc cref="UpdateRefreshToken(RefreshToken, CancellationToken)"/>
        Task UpdateRefreshToken(RefreshToken token) =>
            UpdateRefreshToken(token, CancellationToken.None);

        /// <inheritdoc cref="ExistsById(string, CancellationToken)"/>
        Task ExistsById(string userId) =>
            ExistsById(userId, CancellationToken.None);

        /// <inheritdoc cref="ExistsByEmail(string, CancellationToken)"/>
        Task ExistsByEmail(string userEmail) =>
            ExistsByEmail(userEmail, CancellationToken.None);

        #endregion
    }
}