using System;

namespace GoFla.API.DTOs.Cart;

public record AddToCartDto
{
    public int MenuItemId { get; init; }
    public int Quantity { get; init; }
    public string? SpecialInstructions { get; init; }
}
