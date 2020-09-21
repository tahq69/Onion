using Microsoft.AspNetCore.Identity;

namespace Onion.Identity.Models
{
    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName)
            : base(roleName)
        {
        }
    }
}