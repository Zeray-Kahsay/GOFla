using System;

namespace GoFla.API.DTOs.Restaurants;

public record RestaurantDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public decimal DeliveryFee { get; init; }
    public int EstimatedDeliveryTime { get; init; }
    public bool IsActive { get; init; }
}
