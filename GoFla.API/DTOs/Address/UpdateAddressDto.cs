using System;

namespace GoFla.API.DTOs.Address;

public record UpdateAddressDto
{
    public string Label { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string  CountryCode  { get; set; } = string.Empty;
    public string  PostalCode  { get; set; } = string.Empty;

    public double  Latitude  { get; init; }
    public double  Longitude  { get; init; }
    
    public bool IsDefault { get; init; }
}
