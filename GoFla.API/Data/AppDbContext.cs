using GoFla.API.Domain;
using GoFla.API.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Restaurant> Restaurants { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ReviewResponse> ReviewResponses { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<DeliveryZone> DeliveryZones => Set<DeliveryZone>();



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // User configuration
        builder.Entity<User>(entity =>
        {
            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.Addresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // User and user-roles config 

        builder.Entity<User>()
                .HasMany(au => au.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

        builder.Entity<AppRole>()
               .HasMany(ap => ap.UserRoles)
               .WithOne(ur => ur.Role)
               .HasForeignKey(ar => ar.RoleId)
               .IsRequired();

        // Restaurant configuration
        builder.Entity<Restaurant>(entity =>
        {
            entity.Property(r => r.DeliveryFee)
                .HasColumnType("decimal(18,2)");

            entity.HasMany(r => r.MenuItems)
                .WithOne(m => m.Restaurant)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Address)
                  .WithOne(a => a.Restaurant)
                .HasForeignKey<Address>(a => a.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Owner)
                  .WithMany()
                  .HasForeignKey(r => r.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Category configuration
        builder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.HasIndex(c => new {c.RestaurantId, c.Name})
                .IsUnique();
            
            entity.HasMany(c => c.MenuItems)
                .WithOne(m => m.Category)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Address config
        builder.Entity<Address>(entity =>
        {
            entity.HasOne(a => a.User)
                  .WithMany(u => u.Addresses)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Order config
        builder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);

            entity.HasOne(oi => oi.Order)
                  .WithMany(o => o.Items)
                  .HasForeignKey(oi => oi.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(oi => oi.MenuItem)
                  .WithMany(mi => mi.OrderItems)
                  .HasForeignKey(oi => oi.MenuItemId)
                  .OnDelete(DeleteBehavior.Restrict);

        });

        // MenuItem configuration
        builder.Entity<MenuItem>(entity =>
        {
            entity.Property(m => m.Price)
                .HasPrecision(10, 2);

            entity.HasIndex(m => m.RestaurantId); // for querying menu items by restaurant
            entity.HasIndex(m => m.Category); // for querying menu items by category

            entity.HasOne(m => m.Restaurant)
                .WithMany(r => r.MenuItems)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Cart configuration
        builder.Entity<Cart>(entity =>
        {
            entity.HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Review configuration
        builder.Entity<Review>(entity =>
        {
            entity.HasIndex(r => r.RestaurantId);
            entity.HasIndex(r => r.UserId);
            entity.HasIndex(r => new { r.RestaurantId, r.UserId });

            entity.Property(r => r.Rating)
                .IsRequired();

            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Restaurant)
                .WithMany()
                .HasForeignKey(r => r.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Order)
                .WithMany()
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(r => r.Responses)
                .WithOne(rr => rr.Review)
                .HasForeignKey(rr => rr.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Favorite configuration
        builder.Entity<Favorite>(entity =>
        {
            entity.HasIndex(f => new { f.UserId, f.RestaurantId })
            .IsUnique();

            entity.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.Restaurant)
                .WithMany()
                .HasForeignKey(f => f.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ReviewResponse configuration
        builder.Entity<ReviewResponse>(entity =>
        {
            entity.HasOne(rr => rr.Responder)
              .WithMany()
              .HasForeignKey(rr => rr.ResponderId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        // Order configuration
        builder.ApplyConfiguration(new OrderConfiguration());

        // OrderItem configuration
        builder.ApplyConfiguration(new OrderItemConfiguration());
    }



}

