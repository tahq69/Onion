using Microsoft.AspNetCore.Identity;
using Onion.Application.Enums;
using Onion.Identity.Models;
using System.Threading.Tasks;

namespace Onion.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Roles.SuperAdmin.ToString()))
                await roleManager.CreateAsync(new ApplicationRole(Roles.SuperAdmin.ToString()) { Id = "d674e95b-d52d-4ae4-93a4-e30cc93b1c23" });

            if (!await roleManager.RoleExistsAsync(Roles.Admin.ToString()))
                await roleManager.CreateAsync(new ApplicationRole(Roles.Admin.ToString()) { Id = "0d58228d-1405-456d-b02f-a7024d306953" });

            if (!await roleManager.RoleExistsAsync(Roles.Moderator.ToString()))
                await roleManager.CreateAsync(new ApplicationRole(Roles.Moderator.ToString()) { Id = "78ece2ec-9b7f-4fd5-b2c5-c2ae7ab036ed" });

            if (!await roleManager.RoleExistsAsync(Roles.Basic.ToString()))
                await roleManager.CreateAsync(new ApplicationRole(Roles.Basic.ToString()) { Id = "dcc064f6-1dc9-40f1-ab54-9ce262ad1f22" });
        }
    }
}