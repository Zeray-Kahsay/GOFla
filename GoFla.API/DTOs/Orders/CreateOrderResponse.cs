using System;

namespace GoFla.API.DTOs.Orders;

public record CreateOrderResponse
{
    public int  OrderId  { get; set; }
    public string  ClientSecret  { get; set; } = string.Empty;
}
