using System;
using GoFla.API.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Data;

public static class DbInitializer
{
    public static async Task Seed(UserManager<User> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var roles = new List<AppRole>
        {
            new(){Name = "Admin"},
            new(){Name = "Customer"}
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }

        // Seed admin
        var admin = new User
        {
            FirstName = "Administrator",
            LastName = "Administrator",
            UserName = "admin@gmail.com",
            Email = "admin@gmail.com",
            CreatedAt = DateTime.UtcNow,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        if (await userManager.FindByEmailAsync(admin.Email) == null)
        {
            await userManager.CreateAsync(admin, "Pa$$w0rd1234");
            await userManager.AddToRolesAsync(admin, new[]{"Admin", "Customer"});
        }
        
    }
}
