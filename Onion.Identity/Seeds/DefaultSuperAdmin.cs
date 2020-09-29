using Microsoft.AspNetCore.Identity;
using Onion.Application.Enums;
using Onion.Identity.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Onion.Identity.Seeds
{
    /// <summary>
    /// Super-admin user seeder.
    /// </summary>
    public static class DefaultSuperAdmin
    {
        /// <summary>
        /// Seed super-admin user.
        /// </summary>
        /// <param name="userManager">Application user manager.</param>
        /// <param name="roleManager">Application role manager.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            var defaultUser = new ApplicationUser
            {
                Id = "63a4a47f-8e94-4650-b7a3-324f5f799ad8",
                UserName = "superadmin",
                Email = "superadmin@gmail.com",
                FirstName = "Mukesh",
                LastName = "Murugan",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "P@ssw0rd!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Moderator.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                }
            }
        }
    }
}