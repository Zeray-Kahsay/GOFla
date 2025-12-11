using System;
using GoFla.API.Commons;

namespace GoFla.API.Services;

public interface IStripeService
{
    Task<Result<string>> CreatePaymentIntentAsync(decimal amount, string userId, string? paymentMehtodId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RefundPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ConfirmPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
}
