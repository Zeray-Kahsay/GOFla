using GoFla.API.Domain;
using GoFla.API.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Restaurant> Restaurants { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

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
        });

        // MenuItem configuration
        builder.Entity<MenuItem>(entity =>
        {
            entity.Property(m => m.Price)
                .HasColumnType("decimal(18,2)");

            entity.HasIndex(m => m.RestaurantId);
            entity.HasIndex(m => m.Category);
        });

        // Cart configuration
        builder.Entity<Cart>(entity =>
        {
            entity.HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Order configuration
        builder.ApplyConfiguration(new OrderConfiguration());

        // OrderItem configuration
        builder.ApplyConfiguration(new OrderItemConfiguration());
    }

}

