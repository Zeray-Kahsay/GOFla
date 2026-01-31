using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Payment;

namespace GoFla.API.Services;

public interface IStripeService
{
    Task<Result<StripeIntentResult>> CreatePaymentIntentAsync(decimal amount, string currency, string orderNumber,  CancellationToken cancellationToken = default);
    Task<Result<bool>> RefundPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ConfirmPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
    Task<Result<string>> CreateCustomerAsync(string email, string name, string userId, CancellationToken cancellationToken = default);
    Task<Result<List<PaymentMethodDto>>> GetCustomerPaymentMethodsAsync(string customerId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AttachPaymentMethodToCustomerAsync(string paymentMethodId, string customerId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DetachPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CancelPaymentIntentAsync(string paymentIntentId, CancellationToken ct = default);

}
