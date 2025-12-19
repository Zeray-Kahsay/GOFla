using System;

namespace GoFla.API.Domain;

public class Address
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty; // e.g., "Home", "Work"
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public  User User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
}
