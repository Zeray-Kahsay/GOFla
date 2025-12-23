using System;

namespace GoFla.API.DTOs.Search;

public record RestaurantSearchResultDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public decimal DeliveryFee { get; init; }
    public int EstimatedDeliveryTime { get; init; }
    public double AverageRating { get; init; }
    public int ReviewCount { get; init; }
    public bool IsFavorite { get; init; }
}
