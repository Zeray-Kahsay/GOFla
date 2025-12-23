using System;

namespace GoFla.API.Domain;

public class Favorite
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public Restaurant Restaurant { get; set; } = null!;
    public int RestaurantId { get; set; }
}
