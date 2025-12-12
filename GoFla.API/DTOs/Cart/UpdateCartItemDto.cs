using System;

namespace GoFla.API.DTOs.Cart;

public record UpdateCartItemDto
{
    public int  Quantity { get; set; }
}
