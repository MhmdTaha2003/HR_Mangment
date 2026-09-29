using HR.Application.Common.Security;
using Microsoft.AspNetCore.Identity;

namespace HR.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in AppRoles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var createResult = await roleManager.CreateAsync(
                new IdentityRole(roleName));

            if (!createResult.Succeeded &&
                !await roleManager.RoleExistsAsync(roleName))
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}': {FormatErrors(createResult)}");
            }
        }
    }

    public static async Task SeedAdminAsync(
        UserManager<ApplicationUser> userManager,
        string adminEmail,
        string adminPassword)
    {
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult =
                await userManager.CreateAsync(admin, adminPassword);

            if (!createResult.Succeeded)
            {
                admin = await userManager.FindByEmailAsync(adminEmail);

                if (admin is null)
                {
                    throw new InvalidOperationException(
                        $"Failed to create admin user: {FormatErrors(createResult)}");
                }
            }
        }

        if (!await userManager.IsInRoleAsync(admin, AppRoles.Admin))
        {
            var roleResult = await userManager.AddToRoleAsync(
                admin,
                AppRoles.Admin);

            if (!roleResult.Succeeded &&
                !await userManager.IsInRoleAsync(admin, AppRoles.Admin))
            {
                throw new InvalidOperationException(
                    $"Failed to assign the Admin role to '{adminEmail}': {FormatErrors(roleResult)}");
            }
        }
    }

    private static string FormatErrors(IdentityResult result) =>
        string.Join(", ", result.Errors.Select(error =>
            $"{error.Code}: {error.Description}"));
}
