using E_COM_Models;
using E_EOM_Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace E_COM_DataAccess.Data
{
    /// <summary>
    /// Runs once at application startup.
    /// Ensures the standard roles exist, and seeds one default Admin account
    /// if no Admin exists yet (solves the "no one can create the first admin" problem,
    /// since the Register page only shows the Role dropdown to users who are already Admins).
    /// </summary>
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var role in new[] { SD.Role_Admin, SD.Role_Individual, SD.Role_Employee, SD.Role_Company })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminAlreadyExists = (await userManager.GetUsersInRoleAsync(SD.Role_Admin)).Any();
            if (adminAlreadyExists)
            {
                return;
            }

            const string defaultAdminEmail = "admin@pustakghar.com";
            const string defaultAdminPassword = "Admin@123";

            var existingUser = await userManager.FindByEmailAsync(defaultAdminEmail);
            if (existingUser == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = defaultAdminEmail,
                    Email = defaultAdminEmail,
                    EmailConfirmed = true,
                    Name = "PustakGhar Admin"
                };

                var result = await userManager.CreateAsync(adminUser, defaultAdminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
                }
            }
            else
            {
                // Email exists but somehow has no Admin role yet — grant it instead of creating a duplicate.
                await userManager.AddToRoleAsync(existingUser, SD.Role_Admin);
            }
        }
    }
}
