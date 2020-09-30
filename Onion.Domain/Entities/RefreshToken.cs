using System;

namespace Onion.Domain.Entities
{
    /// <summary>
    /// User refresh token entity.
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Gets or sets refresh token record identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets authentication refresh token value.
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// Gets or sets datetime when record will expire.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Gets a value indicating whether token is expired.
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= Expires;

        /// <summary>
        /// Gets or sets datetime when record was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets creator IP address.
        /// </summary>
        public string? CreatedByIp { get; set; }

        /// <summary>
        /// Gets or sets datetime when record was deleted.
        /// </summary>
        public DateTime? Revoked { get; set; }

        /// <summary>
        /// Gets or sets deleter IP address.
        /// </summary>
        public string? RevokedByIp { get; set; }

        /// <summary>
        /// Gets or sets replacement token.
        /// </summary>
        public string? ReplacedByToken { get; set; }

        /// <summary>
        /// Gets a value indicating whether record is active.
        /// </summary>
        public bool IsActive => Revoked == null && !IsExpired;
    }
}