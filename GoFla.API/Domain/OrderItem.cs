using System;

namespace GoFla.API.Domain;

public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; } // Price at the time of order
    public string? SpecialInstructions { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public int OrderId { get; set; }
    
    public MenuItem MenuItem { get; set; } = null!;
    public int MenuItemId { get; set; }
}
