using System;

namespace GoFla.API.DTOs.Search;

public record MenuItemSearchResultDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Category { get; init; } = string.Empty;
    public int RestaurantId { get; init; }
    public string RestaurantName { get; init; } = string.Empty;
}
