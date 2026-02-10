using System;
using GoFla.API.Domain;

namespace GoFla.API.DTOs.Payment;

public record CreatePaymentIntentRequest
{
    public string OrderNumber { get; set; } = string.Empty;
    public PaymentProvider Provider { get; set; } = PaymentProvider.Stripe;  // vipps | card | paypal etc.
    public string  Method  { get; set; } = "card"; // e.g. for card: "card", for vipps: "vipps", for paypal: "paypal"

}
