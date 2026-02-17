using System;

namespace GoFla.API.DTOs.Orders;

public record CreateOrderRequest
{
    public int RestaurantId { get; set; }
    public OrderAddressDto Address { get; set; }
   
}
