using System;

namespace GoFla.API.DTOs.MenuItems;

public record CreateMenuItemDto
{
    public int RestaurantId { get; init; }

    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Category { get; init; } = string.Empty;
    public bool IsAvailable  { get; set; } = true;
}
