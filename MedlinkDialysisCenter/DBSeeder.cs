using Microsoft.AspNetCore.Identity;

public class DbSeeder
{
    public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Create roles
        string[] roles = { "Admin", "Nurse" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Create default Admin user
        var adminEmail = "admin@medlink.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var user = new IdentityUser { UserName = adminEmail, Email = adminEmail };
            await userManager.CreateAsync(user, "Admin@12345");
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}