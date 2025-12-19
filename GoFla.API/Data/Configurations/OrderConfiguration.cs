using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFla.API.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Primary Key
        builder.HasKey(o => o.Id);

        // Properties
        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(o => o.SubTotal)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);

        builder.Property(o => o.DeliveryFee)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);

        builder.Property(o => o.Tax)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasDefaultValue(OrderStatus.Pending);

        builder.Property(o => o.PaymentStatus)
            .HasConversion<string>()
            .HasDefaultValue(PaymentStatus.Pending);

        builder.Property(o => o.StripePaymentIntentId)
            .HasMaxLength(255);

        builder.Property(o => o.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.HasIndex(o => o.UserId);

        builder.HasIndex(o => o.RestaurantId);

        builder.HasIndex(o => o.DeliveryAddressId);

        builder.HasIndex(o => o.CreatedAt);

        builder.HasIndex(o => o.Status);

        builder.HasIndex(o => o.PaymentStatus);

        // Relationships
        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Orders_User");

        builder.HasOne(o => o.Restaurant)
            .WithMany()
            .HasForeignKey(o => o.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Orders_Restaurant");

        builder.HasOne(o => o.DeliveryAddress)
            .WithMany()
            .HasForeignKey(o => o.DeliveryAddressId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Orders_DeliveryAddress");

        // Note: Order -> OrderItems relationship is configured in OrderItemConfiguration
        // to avoid duplicate configuration conflicts

        // Table name
        builder.ToTable("Orders");
    }
}
