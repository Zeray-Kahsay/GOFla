using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Payment;

namespace GoFla.API.Services;

public interface IPaymentService
{
    Task<Result<CreatePaymentIntentResponse>> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken ct);
}
