using System;

namespace GoFla.API.DTOs.Cart;

public record CartDto
{
    public int Id { get; init; }
    public int  RestaurantId { get; set; }
    public List<CartItemDto> Items { get; init; } = new();
    public decimal SubTotal { get; init; }
    public int TotalItems { get; init; }
}
