using System;

namespace GoFla.API.DTOs.Payment;

public record CreatePaymentIntentResponse
{
    public string  ClientSecret  { get; set; } = string.Empty;
    public decimal  Amount  { get; set; } 
    public string  Currency  { get; set; } = string.Empty;
}
