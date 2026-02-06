using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Payment;

namespace GoFla.API.Services;

public interface IPaymentGateway
{
    Task<Result<CreatePaymentResult>> CreatePaymentIntentAsync(Order order, CancellationToken ct = default);
    Task<Result<bool>> CancelPaymentAsync(string externalPaymentId, CancellationToken ct = default);
    Task<Result<bool>> RefundPaymentAsync(string externalPaymentId, CancellationToken ct = default);
    
}
