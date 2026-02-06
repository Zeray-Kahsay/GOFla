using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFla.API.Data.FluentConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);

        // Basic properties
        builder.Property(o => o.OrderNumber)
             .IsRequired()
             .HasMaxLength(50);

        builder.Property(o => o.SubTotal).HasPrecision(18, 2);
        builder.Property(o => o.DeliveryFee).HasPrecision(18, 2);
        builder.Property(o => o.Tax).HasPrecision(18, 2);
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);

        builder.Property(o => o.ExternalPaymentId).HasMaxLength(255);

        builder.Property(o => o.Status).HasConversion<string>();

        builder.Property(o => o.PaymentStatus).HasConversion<string>();
        builder.Property(o => o.PaymentProvider).HasConversion<string>();

        builder.Property(o => o.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        // INDEXES
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.RestaurantId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.PaymentStatus);
        builder.HasIndex(o => o.CreatedAt);

        // RELATIONSHIPS -- ROOT ONLY
        builder.HasOne(o => o.Customer)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Restaurant)
            .WithMany(r => r.Orders)
            .HasForeignKey(o => o.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);

        // OWNED: DELIVERY ADDRESS SNAPSHOT
        builder.OwnsOne(o => o.DeliveryAddressSnapshot, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200);
            address.Property(a => a.City).HasMaxLength(100);
            address.Property(a => a.PostalCode).HasMaxLength(20);
            address.Property(a => a.CountryCode).HasMaxLength(10);

            address.Property(a => a.Latitude);
            address.Property(a => a.Longitude);
        });

        // OWNDED: ORDER ITEMS
        builder.OwnsMany(o => o.Items, item =>
        {
            item.ToTable("OrderItems");

            item.WithOwner().HasForeignKey("OrderId");

            item.Property<int>("Id"); // shadow PK
            item.HasKey("Id");

            item.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(150);

            item.Property(i => i.UnitPrice).HasPrecision(18, 2);
            item.Property(i => i.TotalPrice).HasPrecision(18, 2);

            item.Property(i => i.Quantity);
            item.Property(i => i.SpecialInstructions).HasMaxLength(500);
            item.Property(i => i.MenuItemId); // reference only

        });
    }

}
