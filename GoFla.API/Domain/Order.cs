using System;

namespace GoFla.API.Domain;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public Restaurant Restaurant { get; set; } = null!;
    public int RestaurantId { get; set; }
    public Address DeliveryAddress { get; set; } = null!;
    public int DeliveryAddressId { get; set; }
    public ICollection<OrderItem> Items { get; set; } = [];
}
