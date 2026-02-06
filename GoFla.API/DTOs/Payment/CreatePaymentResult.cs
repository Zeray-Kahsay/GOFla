using System;
using GoFla.API.Domain;

namespace GoFla.API.DTOs.Payment;

public record CreatePaymentResult(string ExternalPaymentId, string ClientSecret, PaymentProvider Provider);

