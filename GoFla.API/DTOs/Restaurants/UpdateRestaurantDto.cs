using System;
using GoFla.API.DTOs.Address;

namespace GoFla.API.DTOs.Restaurants;

public record UpdateRestaurantDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public AddressDto AddressDto { get; init; } = null!;
    public string Phone { get; init; } = string.Empty;
    public decimal DeliveryFee { get; init; }
    public int EstimatedDeliveryTime { get; init; }
}
