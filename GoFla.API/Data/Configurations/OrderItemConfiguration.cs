using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoFla.API.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Primary Key
        builder.HasKey(oi => oi.Id);

        // Properties
        builder.Property(oi => oi.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(oi => oi.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(oi => oi.SpecialInstructions)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(oi => oi.OrderId);

        builder.HasIndex(oi => oi.MenuItemId);

        builder.HasIndex(oi => new { oi.OrderId, oi.MenuItemId })
            .IsUnique();

        // Relationships
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_OrderItems_Order");

        builder.HasOne(oi => oi.MenuItem)
            .WithMany(mi => mi.OrderItems)
            .HasForeignKey(oi => oi.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_OrderItems_MenuItem");

        // Table name
        builder.ToTable("OrderItems");
    }
}
