using System;

namespace GoFla.API.DTOs.Search;

public record SearchResultDto
{
    public List<RestaurantSearchResultDto> Restaurants { get; init; } = new();
    public List<MenuItemSearchResultDto> MenuItems { get; init; } = new();
    public int TotalResults { get; init; }
}
