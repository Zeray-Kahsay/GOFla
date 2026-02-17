using System;
using GoFla.API.Domain;

namespace GoFla.API.DTOs.Payment;

public record CreatePaymentResult
{
    public string ExternalPaymentId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public PaymentProvider Provider { get; init; }

 
}

