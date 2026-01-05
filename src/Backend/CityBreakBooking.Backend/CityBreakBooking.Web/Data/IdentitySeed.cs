using Microsoft.AspNetCore.Identity;

namespace CityBreakBooking.Web.Data;

public static class IdentitySeed
{
    public const string AdminRole = "Admin";
    public const string TravelAgentRole = "TravelAgent";
    //public const string CustomerRole = "Customer";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Roles
        await EnsureRoleAsync(roleManager, AdminRole);
        await EnsureRoleAsync(roleManager, TravelAgentRole);
        //await EnsureRoleAsync(roleManager, CustomerRole);

        // Admin user 
        var adminEmail = "admin@citybreak.com";
        var adminPassword = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception("Failed to create admin: " + errors);
            }
        }

        // ensure admin role
        if (!await userManager.IsInRoleAsync(admin, AdminRole))
            await userManager.AddToRoleAsync(admin, AdminRole);
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create role {roleName}: {errors}");
            }
        }
    }
}