using System;

namespace GoFla.API.DTOs.Favorites;

public record FavoriteDto
{
    public int Id { get; init; }
    public int RestaurantId { get; init; }
    public string RestaurantName { get; init; } = string.Empty;
    public string RestaurantImage { get; init; } = string.Empty;
    public string RestaurantAddress { get; init; } = string.Empty;
    public decimal DeliveryFee { get; init; }
    public DateTime CreatedAt { get; init; }
}
