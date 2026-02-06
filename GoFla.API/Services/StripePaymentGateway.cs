using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Payment;
using Stripe;

namespace GoFla.API.Services;

public class StripePaymentGateway : IPaymentGateway
{
    public async Task<Result<CreatePaymentResult>> CreatePaymentIntentAsync(Order order, CancellationToken ct = default)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(order.TotalAmount * 100),
            Currency = "nok",
            Metadata = new Dictionary<string, string>
            {
                {"orderNumber", order.OrderNumber}
            }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options, cancellationToken: ct);

        return Result<CreatePaymentResult>.Success(
            new CreatePaymentResult(
                intent.Id,
                intent.ClientSecret,
                PaymentProvider.Stripe
            )
        );
    }


    public async Task<Result<bool>> CancelPaymentAsync(string externalPaymentId, CancellationToken ct = default)
    {
        var service = new PaymentIntentService();
        await service.CancelAsync(externalPaymentId, cancellationToken: ct);
        
        return Result<bool>.Success(true);
    }


    public async Task<Result<bool>> RefundPaymentAsync(string externalPaymentId, CancellationToken ct = default)
    {
        var refundService = new RefundService();
        await refundService.CreateAsync(new RefundCreateOptions
        {
            PaymentIntent = externalPaymentId
        }, cancellationToken: ct);

        return Result<bool>.Success(true);
    }
}
