using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Onion.Application.DTOs.Account;
using Onion.Domain.Common;

namespace Onion.Identity.Models
{
    public class ApplicationUser : IdentityUser<string>, IEntity<string>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public List<RefreshToken> RefreshTokens { get; set; } = null!;

        public bool OwnsToken(string token)
        {
            return RefreshTokens?.Any(x => x.Token == token) ?? false;
        }
    }
}