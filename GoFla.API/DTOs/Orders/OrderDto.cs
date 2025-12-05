using System;
using GoFla.API.DTOs.Address;

namespace GoFla.API.DTOs.Orders;

public record OrderDto
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string RestaurantName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal SubTotal { get; init; }
    public decimal DeliveryFee { get; init; }
    public decimal Tax { get; init; }
    public decimal TotalAmount { get; init; }
    public string PaymentStatus { get; init; } = string.Empty;
    public AddressDto DeliveryAddress { get; init; } = null!;
    public List<OrderItemDto> Items { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}
