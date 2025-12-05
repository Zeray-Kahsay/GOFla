using System;

namespace GoFla.API.DTOs.MenuItems;

public record MenuItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public bool IsAvailable { get; init; }
    public int RestaurantId { get; init; }
    public string RestaurantName { get; init; } = string.Empty;
}
