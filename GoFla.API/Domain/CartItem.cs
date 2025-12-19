using System;

namespace GoFla.API.Domain;

public class CartItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Cart Cart { get; set; } = null!;
    public int CartId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
    public int MenuItemId { get; set; }
}
