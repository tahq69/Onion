using System;

namespace Onion.Domain.Settings
{
    /// <summary>
    /// JWT service settings.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets jWT security key.
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// Gets or sets jWT issuer.
        /// </summary>
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// Gets or sets jWT audience.
        /// </summary>
        public string Audience { get; set; } = null!;

        /// <summary>
        /// Gets or sets jWT token duration.
        /// </summary>
        public TimeSpan Duration { get; set; }
    }
}