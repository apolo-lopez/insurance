using Microsoft.AspNetCore.Identity;

namespace Evaluation.API.Seed
{
    public class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider service, IConfiguration configuration)
        {
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = service.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = new[] { "Admin", "Client" };

            foreach (var item in roles)
            {
                if (!await roleManager.RoleExistsAsync(item))
                {
                    await roleManager.CreateAsync(new IdentityRole(item));
                }
            }

            // ---------------------------------------------
            // Create admin user if it does not exist
            // ---------------------------------------------
            var adminEmail = configuration["Seed:AdminEmail"] ?? "admin@system.com";
            var adminPassword = configuration["Seed:AdminPassword"] ?? "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);

                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception($"Error creating admin user: {string.Join(", ", createAdmin.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // Ensure admin has Admin role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
