using Microsoft.AspNetCore.Identity;

namespace Onion.Identity.Models
{
    /// <summary>
    /// Application role entity.
    /// </summary>
    public class ApplicationRole : IdentityRole<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRole"/> class.
        /// </summary>
        public ApplicationRole()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRole"/> class.
        /// </summary>
        /// <param name="roleName">Application role name.</param>
        public ApplicationRole(string roleName)
            : base(roleName)
        {
        }
    }
}