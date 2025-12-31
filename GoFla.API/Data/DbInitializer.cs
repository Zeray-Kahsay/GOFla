using System;
using GoFla.API.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Data;

public static class DbInitializer
{
  public static async Task Seed(UserManager<User> userManager, RoleManager<AppRole> roleManager, AppDbContext context)
  {
    if (!await userManager.Users.AnyAsync())
    {

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
    }

    if (!await context.Restaurants.AnyAsync() || !await context.Addresses.AnyAsync())
    {
      var addresses = new List<Address>
            {
               new Address
            {
                Label = "Main Branch",
                Street = "Karl Johans gate 10",
                City = "Oslo",
                State = "Oslo",
                PostalCode = "0154",
                CountryCode = "NO",
                Latitude = 59.9111,
                Longitude = 10.7528,
                IsDefault = true,
                CreatedAt = DateTime.UtcNow,
                UserId = null,
            },
            new Address
            {
                Label = "Downtown Branch",
                Street = "Dronning Eufemias gate 30",
                City = "Oslo",
                State = "Oslo",
                PostalCode = "0191",
                CountryCode = "NO",
                Latitude = 59.9115,
                Longitude = 10.7575,
                IsDefault = true,
                CreatedAt = DateTime.UtcNow,
                UserId = null,
            },
            new Address
            {
              Label = "Storo Branch",
              Street = "Nydalenveie gate 30",
              City = "Oslo",
              State = "Oslo",
              CountryCode = "NO",
              Latitude = 59.9110,
              Longitude = 10.8482,
              IsDefault = true,
              CreatedAt = DateTime.UtcNow,
              UserId = null,
            },
            new Address
            {
              Label = "Byporten Branch",
              Street = "Byporten gate 3",
              City = "Oslo",
              State = "Oslo",
              CountryCode = "NO",
              Latitude = 59.9330,
              Longitude = 10.8481,
              IsDefault = true,
              CreatedAt = DateTime.UtcNow,
              UserId = null,
            },
             new Address
            {
              Label = "Storgate Branch",
              Street = "Stor gate 30",
              City = "Oslo",
              State = "Oslo",
              CountryCode = "NO",
              Latitude = 59.8110,
              Longitude = 10.7482,
              IsDefault = true,
              CreatedAt = DateTime.UtcNow,
              UserId = null,
            },
              new Address
            {
              Label = "Furuset Branch",
              Street = "Furuset gate 30",
              City = "Oslo",
              State = "Oslo",
              CountryCode = "NO",
              Latitude = 59.7110,
              Longitude = 10.9482,
              IsDefault = true,
              CreatedAt = DateTime.UtcNow,
              UserId = null,
            },
              new Address
            {
              Label = "Bergen Branch",
              Street = "Nydalenveie gate 30",
              City = "Bergen",
              State = "Bergen",
              CountryCode = "NO",
              Latitude = 99.9110,
              Longitude = 40.8482,
              IsDefault = true,
              CreatedAt = DateTime.UtcNow,
              UserId = null,
            },

            };
      var restaurants = new List<Restaurant>
    {
          new Restaurant
        {
          Name = "La Piazza",
          Description = "Authentic Neapolitan pizza and classic pastas.",
          ImageUrl = "https://example.com/images/la-piazza.jpg",
          Address = addresses[0],
          Phone = "+1-555-0101",
          DeliveryFee = 2.50m,
          EstimatedDeliveryTime = 30,
          DeliveryRadiusKm = 10,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          OwnerId = null,
          MenuItems = new List<MenuItem>
          {
            new() { Name = "Margherita Pizza", Description = "Tomato, mozzarella, basil", Price = 9.99m, ImageUrl = "https://example.com/images/margherita.jpg", Category = "Pizza", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Penne Arrabbiata", Description = "Spicy tomato sauce, garlic, parsley", Price = 8.50m, ImageUrl = "https://example.com/images/penne.jpg", Category = "Pasta", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Tiramisu", Description = "Classic coffee mascarpone dessert", Price = 5.50m, ImageUrl = "https://example.com/images/tiramisu.jpg", Category = "Dessert", IsAvailable = true, CreatedAt = DateTime.UtcNow }
          }
       },
         new Restaurant
        {
          Name = "Habesha Resto",
          Description = "Delicious Habesha Dishes",
          ImageUrl = "https://example.com/images/la-piazza.jpg",
          Address = addresses[1],
          Phone = "+4745454545",
          DeliveryFee = 2.50m,
          EstimatedDeliveryTime = 30,
          DeliveryRadiusKm = 10,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          OwnerId = null,
          MenuItems = new List<MenuItem>
          {
            new() { Name = "Dero Wet", Description = "Chicken with spicy suace", Price = 9.99m, ImageUrl = "https://example.com/images/margherita.jpg", Category = "Pizza", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Tibis", Description = "Meat with Spicy tomato sauce, garlic, parsley", Price = 8.50m, ImageUrl = "https://example.com/images/penne.jpg", Category = "Pasta", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "BeyeAynetu", Description = "Special selected vegitables with medium suace", Price = 5.50m, ImageUrl = "https://example.com/images/tiramisu.jpg", Category = "Dessert", IsAvailable = true, CreatedAt = DateTime.UtcNow }
          }
       },
         new Restaurant
        {
          Name = "Grill Kebab",
          Description = "Kebabs of different type",
          ImageUrl = "https://example.com/images/la-piazza.jpg",
          Address = addresses[2],
          Phone = "+4745464849",
          DeliveryFee = 2.50m,
          EstimatedDeliveryTime = 30,
          DeliveryRadiusKm = 10,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          OwnerId = null,
          MenuItems = new List<MenuItem>
          {
            new() { Name = "Margherita Pizza", Description = "Tomato, mozzarella, basil", Price = 9.99m, ImageUrl = "https://example.com/images/margherita.jpg", Category = "Pizza", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Penne Arrabbiata", Description = "Spicy tomato sauce, garlic, parsley", Price = 8.50m, ImageUrl = "https://example.com/images/penne.jpg", Category = "Pasta", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Tiramisu", Description = "Classic coffee mascarpone dessert", Price = 5.50m, ImageUrl = "https://example.com/images/tiramisu.jpg", Category = "Dessert", IsAvailable = true, CreatedAt = DateTime.UtcNow }
          }
       },
         new Restaurant
        {
          Name = "La Piazza",
          Description = "Authentic Neapolitan pizza and classic pastas.",
          ImageUrl = "https://example.com/images/la-piazza.jpg",
          Address = addresses[3],
          Phone = "+1-555-0101",
          DeliveryFee = 2.50m,
          EstimatedDeliveryTime = 30,
          DeliveryRadiusKm = 10,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          OwnerId = null,
          MenuItems = new List<MenuItem>
          {
            new() { Name = "Margherita Pizza", Description = "Tomato, mozzarella, basil", Price = 9.99m, ImageUrl = "https://example.com/images/margherita.jpg", Category = "Pizza", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Penne Arrabbiata", Description = "Spicy tomato sauce, garlic, parsley", Price = 8.50m, ImageUrl = "https://example.com/images/penne.jpg", Category = "Pasta", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Tiramisu", Description = "Classic coffee mascarpone dessert", Price = 5.50m, ImageUrl = "https://example.com/images/tiramisu.jpg", Category = "Dessert", IsAvailable = true, CreatedAt = DateTime.UtcNow }
          }
       },
         new Restaurant
        {
          Name = "La Piazza",
          Description = "Authentic Neapolitan pizza and classic pastas.",
          ImageUrl = "https://example.com/images/la-piazza.jpg",
          Address = addresses[4],
          Phone = "+1-555-0101",
          DeliveryFee = 2.50m,
          EstimatedDeliveryTime = 30,
          DeliveryRadiusKm = 10,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          OwnerId = null,
          MenuItems = new List<MenuItem>
          {
            new() { Name = "Margherita Pizza", Description = "Tomato, mozzarella, basil", Price = 9.99m, ImageUrl = "https://example.com/images/margherita.jpg", Category = "Pizza", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Penne Arrabbiata", Description = "Spicy tomato sauce, garlic, parsley", Price = 8.50m, ImageUrl = "https://example.com/images/penne.jpg", Category = "Pasta", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new() { Name = "Tiramisu", Description = "Classic coffee mascarpone dessert", Price = 5.50m, ImageUrl = "https://example.com/images/tiramisu.jpg", Category = "Dessert", IsAvailable = true, CreatedAt = DateTime.UtcNow }
          }
       },
         new Restaurant
       {
          Name = "Seoul Spice",
          Description = "Korean comfort food — bibimbap, Korean fried chicken, and more.",
          ImageUrl = "https://example.com/images/seoul-spice.jpg",
          Address = addresses[5],
          Phone = "+1-555-0202",
          DeliveryFee = 3.00m,
          EstimatedDeliveryTime = 35,
          DeliveryRadiusKm = 10,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          OwnerId = null,
          MenuItems = new List<MenuItem>
          {
            new () { Name = "Bibimbap", Description = "Mixed rice, vegetables, egg, gochujang", Price = 10.99m, ImageUrl = "https://example.com/images/bibimbap.jpg", Category = "Rice Bowls", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new () { Name = "Korean Fried Chicken", Description = "Crispy double-fried wings, spicy glaze", Price = 12.50m, ImageUrl = "https://example.com/images/kfc.jpg", Category = "Mains", IsAvailable = true, CreatedAt = DateTime.UtcNow }
          }
       },
       new Restaurant
      {
        Name = "Green Garden",
        Description = "Plant-based bowls, salads and fresh-pressed juices.",
        ImageUrl = "https://example.com/images/green-garden.jpg",
        Address = addresses[6],
        Phone = "+1-555-0303",
        DeliveryFee = 1.99m,
        EstimatedDeliveryTime = 25,
        DeliveryRadiusKm = 10,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        OwnerId = null,
        MenuItems = new List<MenuItem>
        {
            new () { Name = "Falafel Bowl", Description = "Falafel, quinoa, tahini, greens", Price = 9.50m, ImageUrl = "https://example.com/images/falafel.jpg", Category = "Bowls", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new () { Name = "Kale Caesar", Description = "Kale, vegan parmesan, crunchy croutons", Price = 7.99m, ImageUrl = "https://example.com/images/kale-caesar.jpg", Category = "Salads", IsAvailable = true, CreatedAt = DateTime.UtcNow },
            new () { Name = "Green Juice", Description = "Kale, apple, celery, lemon", Price = 4.99m, ImageUrl = "https://example.com/images/green-juice.jpg", Category = "Drinks", IsAvailable = true, CreatedAt = DateTime.UtcNow }
        }
      }
   };
      await context.Addresses.AddRangeAsync(addresses);
      await context.Restaurants.AddRangeAsync(restaurants);
      await context.SaveChangesAsync();

    }





  }
}
