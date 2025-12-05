using System;

namespace GoFla.API.Domain;

public class CartItem
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Cart Cart { get; set; } = null!;
    public virtual MenuItem MenuItem { get; set; } = null!;
}
