using System;

namespace GoFla.API.DTOs.Orders;

public record CreateOrderRequest
{
    public int RestaurantId { get; set; }

    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
