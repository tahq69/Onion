namespace Onion.Application.Interfaces
{
    /// <summary>
    /// Authenticated user service contract.
    /// </summary>
    public interface IAuthenticatedUserService
    {
        /// <summary>
        /// Gets authenticated user identifier.
        /// </summary>
        string? UserId { get; }
    }
}