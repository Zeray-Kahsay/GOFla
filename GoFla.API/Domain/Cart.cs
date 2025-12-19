using System;

namespace GoFla.API.Domain;

public class Cart
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public ICollection<CartItem> Items { get; set; } = [];
}
