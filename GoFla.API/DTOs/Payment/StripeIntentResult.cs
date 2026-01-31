using System;

namespace GoFla.API.DTOs.Payment;

public record StripeIntentResult(string IntentId, string ClientSecret);



