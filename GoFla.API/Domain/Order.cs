using System;

namespace GoFla.API.Domain;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    // WHO
    public string CustomerId { get; set; } = null!;
    public User Customer { get; set; } = null!;

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    // FINANCIAL SNAPSHOT
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }

    // PAYMENT
    public string? PaymentIntentId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    // LIFECYCLE
    public OrderStatus Status { get; set; }

    // DELIVERY SNAPSHOT
    public DeliveryAddressSnapshot DeliveryAddressSnapshot { get; set; } = new();

    // META
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
