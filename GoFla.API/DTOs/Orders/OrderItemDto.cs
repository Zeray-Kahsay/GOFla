using System;

namespace GoFla.API.DTOs.Orders;

public record OrderItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
    public string? SpecialInstructions { get; init; }
}
