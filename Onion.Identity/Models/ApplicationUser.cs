using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Onion.Application.DTOs.Account;
using Onion.Domain.Common;
using Onion.Domain.Entities;

namespace Onion.Identity.Models
{
    /// <summary>
    /// Application user entity.
    /// </summary>
    public class ApplicationUser : IdentityUser<string>, IEntity<string>
    {
        /// <summary>
        /// Gets or sets user first name.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets user last name.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets user refresh token collection table reference.
        /// </summary>
        public List<RefreshToken> RefreshTokens { get; set; } = null!;
    }
}