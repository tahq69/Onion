using System;
using Microsoft.AspNetCore.Identity;
using Onion.Application.Enums;
using Onion.Identity.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Onion.Identity.Seeds
{
    /// <summary>
    /// Unit-test user seeder.
    /// </summary>
    public static class UnitTestUsers
    {
        /// <summary>
        /// Seeds unit-test users to the database.
        /// </summary>
        /// <param name="userManager">Application user manager.</param>
        /// <param name="roleManager">Application role manager.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            for (int i = 1; i < 50; i++)
            {
                // Seed Default User
                var defaultUser = new ApplicationUser
                {
                    UserName = $"unit-test-{i}",
                    Email = $"unit-test-{i}@example.com",
                    FirstName = $"John-{i}",
                    LastName = $"Doe-{i}",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
                {
                    var user = await userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        await userManager.CreateAsync(defaultUser, "P@ssw0rd!");
                        await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
                    }
                }
            }
        }
    }
}