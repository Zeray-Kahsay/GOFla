using System;

namespace GoFla.API.DTOs.Address;

public record CreateAddressDto
{
    public string Label { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
}
