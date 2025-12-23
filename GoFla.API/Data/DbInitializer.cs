using System;
using GoFla.API.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Data;

public static class DbInitializer
{
    public static async Task Seed(UserManager<User> userManager, RoleManager<AppRole> roleManager, AppDbContext context)
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
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Customer" });
        }

        // using GoFla.API.Domain;
        // using System.Collections.Generic;

        var restaurants = new List<Restaurant>
     {
      new Restaurant
     {
        Id = 1,
        Name = "La Piazza",
        Description = "Authentic Neapolitan pizza and classic pastas.",
        ImageUrl = "https://example.com/images/la-piazza.jpg",
        Address = "12 Roma Street, City",
        Phone = "+1-555-0101",
        DeliveryFee = 2.50m,
        EstimatedDeliveryTime = 30,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        MenuItems = new List<MenuItem>
        {
            new() { Id = 1, Name = "Margherita Pizza", Description = "Tomato, mozzarella, basil", Price = 9.99m, ImageUrl = "https://example.com/images/margherita.jpg", Category = "Pizza", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Penne Arrabbiata", Description = "Spicy tomato sauce, garlic, parsley", Price = 8.50m, ImageUrl = "https://example.com/images/penne.jpg", Category = "Pasta", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 3, Name = "Tiramisu", Description = "Classic coffee mascarpone dessert", Price = 5.50m, ImageUrl = "https://example.com/images/tiramisu.jpg", Category = "Dessert", IsAvailable = true, CreatedAt = DateTime.UtcNow }
        }
   },
      new Restaurant
    {
        Id = 2,
        Name = "Seoul Spice",
        Description = "Korean comfort food — bibimbap, Korean fried chicken, and more.",
        ImageUrl = "https://example.com/images/seoul-spice.jpg",
        Address = "88 Han River Ave, City",
        Phone = "+1-555-0202",
        DeliveryFee = 3.00m,
        EstimatedDeliveryTime = 35,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        MenuItems = new List<MenuItem>
        {
            new () { Id = 4, Name = "Bibimbap", Description = "Mixed rice, vegetables, egg, gochujang", Price = 10.99m, ImageUrl = "https://example.com/images/bibimbap.jpg", Category = "Rice Bowls", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new () { Id = 5, Name = "Korean Fried Chicken", Description = "Crispy double-fried wings, spicy glaze", Price = 12.50m, ImageUrl = "https://example.com/images/kfc.jpg", Category = "Mains", IsAvailable = true, CreatedAt = DateTime.UtcNow }
        }
    },
    new Restaurant
    {
        Id = 3,
        Name = "Green Garden",
        Description = "Plant-based bowls, salads and fresh-pressed juices.",
        ImageUrl = "https://example.com/images/green-garden.jpg",
        Address = "5 Market Lane, City",
        Phone = "+1-555-0303",
        DeliveryFee = 1.99m,
        EstimatedDeliveryTime = 25,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        MenuItems = new List<MenuItem>
        {
            new () { Id = 6, Name = "Falafel Bowl", Description = "Falafel, quinoa, tahini, greens", Price = 9.50m, ImageUrl = "https://example.com/images/falafel.jpg", Category = "Bowls", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new () { Id = 7, Name = "Kale Caesar", Description = "Kale, vegan parmesan, crunchy croutons", Price = 7.99m, ImageUrl = "https://example.com/images/kale-caesar.jpg", Category = "Salads", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new () { Id = 8, Name = "Green Juice", Description = "Kale, apple, celery, lemon", Price = 4.99m, ImageUrl = "https://example.com/images/green-juice.jpg", Category = "Drinks", IsAvailable = true, CreatedAt = DateTime.UtcNow }
        }
    }
};

        await context.Restaurants.AddRangeAsync(restaurants);
        await context.SaveChangesAsync();

    }
}
