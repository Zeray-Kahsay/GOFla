using System;

namespace GoFla.API.DTOs.Cart;

public record CartItemDto
{
    public int Id { get; init; }
    public int MenuItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public string? SpecialInstructions { get; init; }
    public decimal ItemTotal { get; init; }
    public string RestaurantName { get; init; } = string.Empty;
}
