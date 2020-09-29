using Microsoft.AspNetCore.Identity;
using Onion.Application.Enums;
using Onion.Identity.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Onion.Identity.Seeds
{
    /// <summary>
    /// Application basic user seeder.
    /// </summary>
    public static class DefaultBasicUser
    {
        /// <summary>
        /// Seed basic user.
        /// </summary>
        /// <param name="userManager">Application user manager.</param>
        /// <param name="roleManager">Application role manager.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            // Seed Default User
            var defaultUser = new ApplicationUser
            {
                Id = "a116e539-83ba-4843-9789-bd713bf4f89d",
                UserName = "basicuser",
                Email = "basicuser@gmail.com",
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
                }
            }
        }
    }
}