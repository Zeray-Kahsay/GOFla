using System;

namespace GoFla.API.Domain;

public class CartItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
    
    // Navigation properties
    public Cart Cart { get; set; } = null!;
    public int CartId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
    public int MenuItemId { get; set; }

    // Menu Item Snapshot
    public string Name { get; set; } = "";
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}
