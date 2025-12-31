using System;

namespace GoFla.API.DTOs.Address;

public record AddressDto
{
    public int Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string  CountryCode  { get; set; } = string.Empty;
    public string  PostalCode  { get; set; } = string.Empty;
    public double? Latitude  { get; set; }
    public double? Longitude  { get; set; }
    public bool IsDefault { get; init; }
}
