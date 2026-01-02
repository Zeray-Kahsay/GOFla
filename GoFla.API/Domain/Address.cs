using System;

namespace GoFla.API.Domain;

public class Address
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = "NO"; // ISO-2

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation prop
    public int? RestaurantId { get; set; }
     public Restaurant? Restaurant { get; set; } = null!;
     public string? UserId { get; set; } 
    public User? User { get; set; } = null!;

}
