using System;
using GoFla.API.DTOs.Address;

namespace GoFla.API.DTOs.Restaurants;

public record RestaurantDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public AddressDto AddressDto { get; init; } = null!;
    public string Phone { get; init; } = string.Empty;
    public decimal DeliveryFee { get; init; }
    public int EstimatedDeliveryTime { get; init; }
    public bool IsActive { get; init; }
    public bool  IsFavorite { get; set; }
}
