using System;

namespace GoFla.API.DTOs.Orders;

public record CreateOrderDto
{
    public int DeliveryAddressId { get; init; }
    public string? PaymentMethodId { get; init; }
}
