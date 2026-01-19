using System;
using GoFla.API.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Data;

public static class DbInitializer
{

  public static async Task SeedDataAsync(
      UserManager<User> userManager,
      RoleManager<AppRole> roleManager,
      AppDbContext context)
  {
    // -----------------------------
    // 1️⃣ Seed Roles & Users
    // -----------------------------
    var roles = new[] { "Admin", "Customer" };
    foreach (var roleName in roles)
    {
      if (!await roleManager.RoleExistsAsync(roleName))
      {
        await roleManager.CreateAsync(new AppRole { Name = roleName });
      }
    }

    // Admin user
    var adminEmail = "admin@gmail.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
      var admin = new User
      {
        FirstName = "Administrator",
        LastName = "Administrator",
        UserName = adminEmail,
        Email = adminEmail,
        CreatedAt = DateTime.UtcNow,
        SecurityStamp = Guid.NewGuid().ToString()
      };
      await userManager.CreateAsync(admin, "Pa$$w0rd1234");
      await userManager.AddToRolesAsync(admin, roles);
    }

    // Other users (owners)
    var ownerEmails = new[]
    {
        "owner@gmail.com",
        "owner1@gmail.com",
        "owner2@gmail.com",
        "owner3@gmail.com",
        "owner14@gmail.com",
        "owner25@gmail.com",
        "owner36@gmail.com",
        "owner47@gmail.com"
    };

    var users = new List<User>();
    foreach (var email in ownerEmails)
    {
      var existing = await userManager.FindByEmailAsync(email);
      if (existing == null)
      {
        var user = new User
        {
          FirstName = "Sara",
          LastName = "Kim",
          UserName = email,
          Email = email,
          CreatedAt = DateTime.UtcNow
        };
        await userManager.CreateAsync(user, "Pa$$w0rd1234");
        await userManager.AddToRoleAsync(user, "Customer");
        users.Add(user);
      }
      else
      {
        users.Add(existing);
      }
    }

    // -----------------------------
    // 2️⃣ Seed Addresses
    // -----------------------------
    if (!await context.Addresses.AnyAsync())
    {
      var addresses = new List<Address>
        {
            new() { Label = "Main Branch", Street = "Karl Johans gate 10", City = "Oslo", State = "Oslo", CountryCode = "NO", Latitude = 59.9111, Longitude = 10.7528, IsDefault = true, CreatedAt = DateTime.UtcNow, User = users[0] },
            new() { Label = "Downtown Branch", Street = "Dronning Eufemias gate 30", City = "Oslo", State = "Oslo", CountryCode = "NO", Latitude = 59.9115, Longitude = 10.7575, IsDefault = true, CreatedAt = DateTime.UtcNow, User = users[1] },
            new() { Label = "Storo Branch", Street = "Nydalenveie gate 30", City = "Oslo", State = "Oslo", CountryCode = "NO", Latitude = 59.9110, Longitude = 10.8482, IsDefault = true, CreatedAt = DateTime.UtcNow, User = users[2] },
            new() { Label = "Byporten Branch", Street = "Byporten gate 3", City = "Oslo", State = "Oslo", CountryCode = "NO", Latitude = 59.9330, Longitude = 10.8481, IsDefault = true, CreatedAt = DateTime.UtcNow, User = users[3] },
            new() { Label = "Storgate Branch", Street = "Stor gate 30", City = "Oslo", State = "Oslo", CountryCode = "NO", Latitude = 59.8110, Longitude = 10.7482, IsDefault = true, CreatedAt = DateTime.UtcNow, User = users[4] },
            new() { Label = "Furuset Branch", Street = "Furuset gate 30", City = "Oslo", State = "Oslo", CountryCode = "NO", Latitude = 59.7110, Longitude = 10.9482, IsDefault = true, CreatedAt = DateTime.UtcNow, User = users[5] },
            new() { Label = "Bergen Branch", Street = "Nydalenveie gate 30", City = "Bergen", State = "Bergen", CountryCode = "NO", Latitude = 59.9110, Longitude = 10.8482, IsDefault = true, CreatedAt = DateTime.UtcNow, User = users[6] }
        };

      await context.Addresses.AddRangeAsync(addresses);
      await context.SaveChangesAsync();

      // -----------------------------
      // 3️⃣ Seed Restaurants
      // -----------------------------
      var restaurants = new List<Restaurant>
        {
            new() { Name = "La Piazza", Description = "Authentic Neapolitan pizza", Address = addresses[0], Phone = "+1-555-0101", DeliveryFee = 2.50m, EstimatedDeliveryTime = 30, DeliveryRadiusKm = 10, IsActive = true, CreatedAt = DateTime.UtcNow, Owner = users[0] },
            new() { Name = "Habesha Resto", Description = "Delicious Habesha Dishes", Address = addresses[1], Phone = "+4745454545", DeliveryFee = 2.50m, EstimatedDeliveryTime = 30, DeliveryRadiusKm = 10, IsActive = true, CreatedAt = DateTime.UtcNow, Owner = users[1] },
            new() { Name = "Grill Kebab", Description = "Kebabs of different type", Address = addresses[2], Phone = "+4745464849", DeliveryFee = 2.50m, EstimatedDeliveryTime = 30, DeliveryRadiusKm = 10, IsActive = true, CreatedAt = DateTime.UtcNow, Owner = users[2] },
            new() { Name = "La Piazza", Description = "Authentic Neapolitan pizza", Address = addresses[3], Phone = "+1-555-0101", DeliveryFee = 2.50m, EstimatedDeliveryTime = 30, DeliveryRadiusKm = 10, IsActive = true, CreatedAt = DateTime.UtcNow, Owner = users[3] },
            new() { Name = "Seoul Spice", Description = "Korean comfort food", Address = addresses[5], Phone = "+1-555-0202", DeliveryFee = 3.00m, EstimatedDeliveryTime = 35, DeliveryRadiusKm = 10, IsActive = true, CreatedAt = DateTime.UtcNow, Owner = users[5] },
            new() { Name = "Green Garden", Description = "Plant-based bowls, salads and fresh-pressed juices", Address = addresses[6], Phone = "+1-555-0303", DeliveryFee = 1.99m, EstimatedDeliveryTime = 25, DeliveryRadiusKm = 10, IsActive = true, CreatedAt = DateTime.UtcNow, Owner = users[6] }
        };

      await context.Restaurants.AddRangeAsync(restaurants);
      await context.SaveChangesAsync();

      // -----------------------------
      // 4️⃣ Seed Categories
      // -----------------------------
      var categoryNames = new[]
      {
            "Pizza","Burger","Sushi","Chinese","Italian","Mexican","Indian","Thai",
            "Vietnamese","American","Desserts","Beverages","Salads","Sandwiches",
            "Seafood","Steak","Vegetarian","Vegan"
        };

      var allCategories = new List<Category>();
      foreach (var restaurant in restaurants)
      {
        allCategories.AddRange(categoryNames.Select((name, index) => new Category
        {
          Name = name,
          Restaurant = restaurant,
          SortOrder = index,
          CreatedAt = DateTime.UtcNow
        }));
      }

      await context.Categories.AddRangeAsync(allCategories);
      await context.SaveChangesAsync();

      // -----------------------------
      // 5️⃣ Seed MenuItems
      // -----------------------------
      var menuItems = new List<MenuItem>
        {
            new() { Name = "Margherita Pizza", Description = "Classic pizza with tomato sauce, mozzarella, basil", Price = 8.99m, Restaurant = restaurants[0], Category = allCategories.First(c => c.RestaurantId == restaurants[0].Id && c.Name == "Pizza"), CreatedAt = DateTime.UtcNow, IsAvailable = true, ImageUrl = "https://example.com/images/margherita-pizza.jpg" },
            new() { Name = "Beef Burger", Description = "Augus beef, cheddar, lettuce, tomato, special sauce", Price = 8.99m, Restaurant = restaurants[1], Category = allCategories.First(c => c.RestaurantId == restaurants[1].Id && c.Name == "Burger"), CreatedAt = DateTime.UtcNow, IsAvailable = true, ImageUrl = "https://example.com/images/beef-burger.jpg" },
            new() { Name = "Vegan Salad Bowl", Description = "Quinoa, chickpeas, avocado, mixed greens, lemon-tahini", Price = 7.99m, Restaurant = restaurants[5], Category = allCategories.First(c => c.RestaurantId == restaurants[5].Id && c.Name == "Vegan"), CreatedAt = DateTime.UtcNow, IsAvailable = true, ImageUrl = "https://example.com/images/vegan-salad-bowl.jpg" }
        };

      await context.MenuItems.AddRangeAsync(menuItems);
      await context.SaveChangesAsync();
    }
  }
}


// public static class DbInitializer
// {
//   public static async Task Seed(UserManager<User> userManager, RoleManager<AppRole> roleManager, AppDbContext context)
//   {

//     if (context.Database.GetAppliedMigrations().Any() == false)
//     {
//       return;
//     }


//     var users = new List<User>();
//     if (!await userManager.Users.AnyAsync())
//     {

//       var roles = new List<AppRole>
//         {
//             new(){Name = "Admin"},
//             new(){Name = "Customer"}
//         };

//       foreach (var role in roles)
//       {
//         if (!await roleManager.RoleExistsAsync(role.Name!))
//         {
//           await roleManager.CreateAsync(role);
//         }
//       }

//       // Seed admin and users
//       var admin = new User
//       {
//         FirstName = "Administrator",
//         LastName = "Administrator",
//         UserName = "admin@gmail.com",
//         Email = "admin@gmail.com",
//         CreatedAt = DateTime.UtcNow,
//         SecurityStamp = Guid.NewGuid().ToString()
//       };


//       if (await userManager.FindByEmailAsync(admin.Email) == null)
//       {
//         await userManager.CreateAsync(admin, "Pa$$w0rd1234");
//         await userManager.AddToRolesAsync(admin, new[] { "Admin", "Customer" });
//       }

//       users = new List<User>
//       {
//         new User
//         {
//           UserName = "owner@gmail.com",
//           Email = "owner@gmail.com",
//           FirstName = "Sara",
//           LastName = "Kim",
//           CreatedAt = DateTime.UtcNow
//         },
//         new User
//         {
//           UserName = "owner1@gmail.com",
//           Email = "owner1@gmail.com",
//           FirstName = "Sara",
//           LastName = "Kim",
//           CreatedAt = DateTime.UtcNow
//         },
//         new User
//         {
//           UserName = "owner2@gmail.com",
//           Email = "owner2@gmail.com",
//           FirstName = "Sara",
//           LastName = "Kim",
//           CreatedAt = DateTime.UtcNow
//         },
//         new User
//         {
//           UserName = "owner3@gmail.com",
//           Email = "owner3@gmail.com",
//           FirstName = "Sara",
//           LastName = "Kim",
//           CreatedAt = DateTime.UtcNow
//         },
//         new User
//         {
//           UserName = "owner14@gmail.com",
//           Email = "owner14@gmail.com",
//           FirstName = "Sara",
//           LastName = "Kim",
//           CreatedAt = DateTime.UtcNow
//         },
//         new User
//         {
//           UserName = "owner25@gmail.com",
//           Email = "owner25@gmail.com",
//           FirstName = "Sara",
//           LastName = "Kim",
//           CreatedAt = DateTime.UtcNow
//         },
//         new User
//         {
//           UserName = "owner36@gmail.com",
//           Email = "owner36@gmail.com",
//           FirstName = "Sara",
//           LastName = "Kim",
//           CreatedAt = DateTime.UtcNow
//         },
//         new User
//         {
//           UserName = "owner47@gmail.com",
//           Email = "owner47@gmail.com",
//           FirstName = "Sara",
//           LastName = "Kim",
//           CreatedAt = DateTime.UtcNow
//         },
//       };

//       foreach (var user in users)
//       {
//         if (await userManager.FindByEmailAsync(user.Email!) == null)
//         {
//           await userManager.CreateAsync(user, "Pa$$w0rd1234");
//           await userManager.AddToRolesAsync(user, new[] { "Customer" });
//         }
//       }
//     }
//     else
//     {
//       // Users already exist - fetch them from database
//       users = await userManager.Users
//         .Where(u => u.UserName != "admin@gmail.com")
//         .OrderBy(u => u.Id)
//         .Take(8)
//         .ToListAsync();
//     }


//     if (!await context.Restaurants.AnyAsync() || !await context.Addresses.AnyAsync())
//     {
//       // Only seed if restaurants don't exist
//       if (await context.Restaurants.AnyAsync())
//       {
//         return; // Exit early if restaurants already exist
//       }
//       var addresses = new List<Address>
//             {
//                new Address
//             {
//                 Label = "Main Branch",
//                 Street = "Karl Johans gate 10",
//                 City = "Oslo",
//                 State = "Oslo",
//                 PostalCode = "0154",
//                 CountryCode = "NO",
//                 Latitude = 59.9111,
//                 Longitude = 10.7528,
//                 IsDefault = true,
//                 CreatedAt = DateTime.UtcNow,
//                 UserId = users[0].Id,
//             },
//             new Address
//             {
//                 Label = "Downtown Branch",
//                 Street = "Dronning Eufemias gate 30",
//                 City = "Oslo",
//                 State = "Oslo",
//                 PostalCode = "0191",
//                 CountryCode = "NO",
//                 Latitude = 59.9115,
//                 Longitude = 10.7575,
//                 IsDefault = true,
//                 CreatedAt = DateTime.UtcNow,
//                 UserId = users[1].Id,
//             },
//             new Address
//             {
//               Label = "Storo Branch",
//               Street = "Nydalenveie gate 30",
//               City = "Oslo",
//               State = "Oslo",
//               CountryCode = "NO",
//               Latitude = 59.9110,
//               Longitude = 10.8482,
//               IsDefault = true,
//               CreatedAt = DateTime.UtcNow,
//               UserId = users[2].Id,
//             },
//             new Address
//             {
//               Label = "Byporten Branch",
//               Street = "Byporten gate 3",
//               City = "Oslo",
//               State = "Oslo",
//               CountryCode = "NO",
//               Latitude = 59.9330,
//               Longitude = 10.8481,
//               IsDefault = true,
//               CreatedAt = DateTime.UtcNow,
//               UserId = users[3].Id,
//             },
//              new Address
//             {
//               Label = "Storgate Branch",
//               Street = "Stor gate 30",
//               City = "Oslo",
//               State = "Oslo",
//               CountryCode = "NO",
//               Latitude = 59.8110,
//               Longitude = 10.7482,
//               IsDefault = true,
//               CreatedAt = DateTime.UtcNow,
//               UserId = users[4].Id,
//             },
//               new Address
//             {
//               Label = "Furuset Branch",
//               Street = "Furuset gate 30",
//               City = "Oslo",
//               State = "Oslo",
//               CountryCode = "NO",
//               Latitude = 59.7110,
//               Longitude = 10.9482,
//               IsDefault = true,
//               CreatedAt = DateTime.UtcNow,
//               UserId = users[5].Id,
//             },
//               new Address
//             {
//               Label = "Bergen Branch",
//               Street = "Nydalenveie gate 30",
//               City = "Bergen",
//               State = "Bergen",
//               CountryCode = "NO",
//               Latitude = 99.9110,
//               Longitude = 40.8482,
//               IsDefault = true,
//               CreatedAt = DateTime.UtcNow,
//               UserId = users[6].Id,
//             },

//             };

//       var categoryNames = new List<string>
//       {
//         "Pizza",
//         "Burger",
//         "Sushi",
//         "Chinese",
//         "Italian",
//         "Mexican",
//         "Indian",
//         "Thai",
//         "Vietnamese",
//         "American",
//         "Desserts",
//         "Beverages",
//         "Salads",
//         "Sandwiches",
//         "Seafood",
//         "Steak",
//         "Vegetarian",
//         "Vegan"
//       };




//       var restaurants = new List<Restaurant>
//     {
//           new Restaurant
//         {
//           Name = "La Piazza",
//           Description = "Authentic Neapolitan pizza and classic pastas.",
//           ImageUrl = "",
//           Address = addresses[0],
//           Phone = "+1-555-0101",
//           DeliveryFee = 2.50m,
//           EstimatedDeliveryTime = 30,
//           DeliveryRadiusKm = 10,
//           IsActive = true,
//           CreatedAt = DateTime.UtcNow,
//           OwnerId = users[0].Id,
//        },
//          new Restaurant
//         {
//           Name = "Habesha Resto",
//           Description = "Delicious Habesha Dishes",
//           ImageUrl = "",
//           Address = addresses[1],
//           Phone = "+4745454545",
//           DeliveryFee = 2.50m,
//           EstimatedDeliveryTime = 30,
//           DeliveryRadiusKm = 10,
//           IsActive = true,
//           CreatedAt = DateTime.UtcNow,
//           OwnerId = users[1].Id,
//        },
//          new Restaurant
//         {
//           Name = "Grill Kebab",
//           Description = "Kebabs of different type",
//           ImageUrl = "",
//           Address = addresses[2],
//           Phone = "+4745464849",
//           DeliveryFee = 2.50m,
//           EstimatedDeliveryTime = 30,
//           DeliveryRadiusKm = 10,
//           IsActive = true,
//           CreatedAt = DateTime.UtcNow,
//           OwnerId = users[2].Id,
//        },
//          new Restaurant
//         {
//           Name = "La Piazza",
//           Description = "Authentic Neapolitan pizza and classic pastas.",
//           ImageUrl = "",
//           Address = addresses[3],
//           Phone = "+1-555-0101",
//           DeliveryFee = 2.50m,
//           EstimatedDeliveryTime = 30,
//           DeliveryRadiusKm = 10,
//           IsActive = true,
//           CreatedAt = DateTime.UtcNow,
//           OwnerId = users[3].Id,
//        },
//          new Restaurant
//         {
//           Name = "La Piazza",
//           Description = "Authentic Neapolitan pizza and classic pastas.",
//           ImageUrl = "",
//           Address = addresses[4],
//           Phone = "+1-555-0101",
//           DeliveryFee = 2.50m,
//           EstimatedDeliveryTime = 30,
//           DeliveryRadiusKm = 10,
//           IsActive = true,
//           CreatedAt = DateTime.UtcNow,
//           OwnerId = users[4].Id,
//        },
//          new Restaurant
//        {
//           Name = "Seoul Spice",
//           Description = "Korean comfort food — bibimbap, Korean fried chicken, and more.",
//           ImageUrl = "",
//           Address = addresses[5],
//           Phone = "+1-555-0202",
//           DeliveryFee = 3.00m,
//           EstimatedDeliveryTime = 35,
//           DeliveryRadiusKm = 10,
//           IsActive = true,
//           CreatedAt = DateTime.UtcNow,
//           OwnerId = users[5].Id,
//        },
//        new Restaurant
//       {
//         Name = "Green Garden",
//         Description = "Plant-based bowls, salads and fresh-pressed juices.",
//         ImageUrl = "",
//         Address = addresses[6],
//         Phone = "+1-555-0303",
//         DeliveryFee = 1.99m,
//         EstimatedDeliveryTime = 25,
//         DeliveryRadiusKm = 10,
//         IsActive = true,
//         CreatedAt = DateTime.UtcNow,
//         OwnerId = users[6].Id,
//       }

//    };

//       var menuItems = new List<MenuItem>();

//       // Save restaurants and addresses first
//       await context.Addresses.AddRangeAsync(addresses);
//       await context.Restaurants.AddRangeAsync(restaurants);
//       await context.SaveChangesAsync();

//       // Now create menu items with correct RestaurantIds (after restaurants are saved)
//       menuItems = new List<MenuItem>
//       {
//         new()
//         {
//         Name = "Margherita Pizza",
//         Description = "Classic pizza with tomato sauce, mozzarella, and fresh basil.",
//         Price = 8.99m,
//         RestaurantId = restaurants[0].Id,
//         CategoryId = 0, // Will be assigned after categories are saved
//         CreatedAt = DateTime.UtcNow,
//         IsAvailable = true,
//         ImageUrl = "https://example.com/images/margherita-pizza.jpg"
//         },
//         new()
//         {
//         Name = "Beef Burger",
//         Description = "Augus beef, cheddar cheese, lettuce, tomato, and special sauce.",
//         Price = 8.99m,
//         RestaurantId = restaurants[1].Id,
//         CategoryId = 0,
//         CreatedAt = DateTime.UtcNow,
//         IsAvailable = true,
//         ImageUrl = "https://example.com/images/beef-burger.jpg"
//         },
//         new()
//         {
//         Name = "Margherita Pizza",
//         Description = "Classic pizza with tomato sauce, mozzarella, and fresh basil.",
//         Price = 8.99m,
//         RestaurantId = restaurants[2].Id,
//         CategoryId = 0,
//         CreatedAt = DateTime.UtcNow,
//         IsAvailable = true,
//         ImageUrl = "https://example.com/images/margherita-pizza.jpg"
//         },
//         new()
//         {
//         Name = "Beef Burger",
//         Description = "Augus beef, cheddar cheese, lettuce, tomato, and special sauce.",
//         Price = 8.99m,
//         RestaurantId = restaurants[3].Id,
//         CategoryId = 0,
//         CreatedAt = DateTime.UtcNow,
//         IsAvailable = true,
//         ImageUrl = "https://example.com/images/beef-burger.jpg"
//         },
//         new()
//         {
//         Name = "Margherita Pizza",
//         Description = "Classic pizza with tomato sauce, mozzarella, and fresh basil.",
//         Price = 8.99m,
//         RestaurantId = restaurants[4].Id,
//         CategoryId = 0,
//         CreatedAt = DateTime.UtcNow,
//         IsAvailable = true,
//         ImageUrl = "https://example.com/images/margherita-pizza.jpg"
//         },
//         new()
//         {
//         Name = "Beef Burger",
//         Description = "Augus beef, cheddar cheese, lettuce, tomato, and special sauce.",
//         Price = 8.99m,
//         RestaurantId = restaurants[5].Id,
//         CategoryId = 0,
//         CreatedAt = DateTime.UtcNow,
//         IsAvailable = true,
//         ImageUrl = "https://example.com/images/beef-burger.jpg"
//         },
//         new()
//         {
//         Name = "Vegan Salad Bowl",
//         Description = "Quinoa, chickpeas, avocado, mixed greens, and lemon-tahini dressing.",
//         Price = 7.99m,
//         RestaurantId = restaurants[6].Id,
//         CategoryId = 0,
//         CreatedAt = DateTime.UtcNow,
//         IsAvailable = true,
//         ImageUrl = "https://example.com/images/vegan-salad-bowl.jpg"
//         }

//       };

//       // Save restaurants and addresses first
//       await context.Addresses.AddRangeAsync(addresses);
//       await context.Restaurants.AddRangeAsync(restaurants);
//       await context.SaveChangesAsync();

//       // Create categories for each restaurant
//       var allCategories = new List<Category>();
//       foreach (var restaurant in restaurants)
//       {
//         foreach (var categoryName in categoryNames)
//         {
//           allCategories.Add(new Category
//           {
//             Name = categoryName,
//             RestaurantId = restaurant.Id,
//             SortOrder = categoryNames.IndexOf(categoryName),
//             CreatedAt = DateTime.UtcNow,
//             UpdatedAt = DateTime.UtcNow
//           });
//         }
//       }

//       await context.Categories.AddRangeAsync(allCategories);
//       await context.SaveChangesAsync();

//       // Update menu items with correct category IDs
//       foreach (var menuItem in menuItems)
//       {
//         string categoryName = menuItem.Name switch
//         {
//           var name when name.Contains("Pizza") => "Pizza",
//           var name when name.Contains("Sushi") => "Sushi",
//           var name when name.Contains("Salad") => "Salads",
//           var name when name.Contains("Vegan") => "Vegan",
//           var name when name.Contains("Kebab") => "Chinese",
//           var name when name.Contains("Pasta") => "Italian",
//           var name when name.Contains("Taco") => "Mexican",
//           var name when name.Contains("Curry") => "Indian",
//           var name when name.Contains("Noodle") => "Thai",
//           var name when name.Contains("Pho") => "Vietnamese",
//           var name when name.Contains("Steak") => "Steak",
//           var name when name.Contains("Dessert") => "Desserts",
//           var name when name.Contains("Drink") => "Beverages",
//           var name when name.Contains("Sandwich") => "Sandwiches",
//           var name when name.Contains("Seafood") => "Seafood",
//           _ => "Burger"
//         };

//         var category = allCategories.FirstOrDefault(c =>
//           c.RestaurantId == menuItem.RestaurantId &&
//           c.Name == categoryName);

//         if (category != null)
//         {
//           menuItem.CategoryId = category.Id;
//         }
//       }

//       await context.MenuItems.AddRangeAsync(menuItems);
//       await context.SaveChangesAsync();

//     }

//   }
// }
