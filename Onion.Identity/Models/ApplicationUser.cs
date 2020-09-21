using Onion.Application.DTOs.Account;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Onion.Identity.Models
{
    public class ApplicationUser : IdentityUser<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
    }
}