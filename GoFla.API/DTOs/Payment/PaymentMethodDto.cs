using System;

namespace GoFla.API.DTOs.Payment;

public record PaymentMethodDto
{
    public string Id { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public string Last4 { get; init; } = string.Empty;
    public long ExpiryMonth { get; init; }
    public long ExpiryYear { get; init; }
}
