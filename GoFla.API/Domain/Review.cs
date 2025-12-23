using System;

namespace GoFla.API.Domain;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; } // 1-5 stars
    public string Title { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Admin moderation
    public bool IsApproved { get; set; } = true;
    public bool IsFlagged { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public Restaurant Restaurant { get; set; } = null!;
    public int RestaurantId { get; set; }
    public Order? Order { get; set; }
    public int? OrderId { get; set; } // Link to order to verify purchase
    public ICollection<ReviewResponse> Responses { get; set; } = [];
}
