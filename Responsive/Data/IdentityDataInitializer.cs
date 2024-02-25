using Microsoft.AspNetCore.Identity;
using Responsive.Data;
using Responsive.Models;

public static class IdentityDataInitializer
{
    public static async Task SeedData(UserManager<Account> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Ensure roles exist
        string[] roleNames = { UserRole.Admin, UserRole.User };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                // Create the role and add it to the database
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
