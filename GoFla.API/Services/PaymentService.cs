using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Payment;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class PaymentService (
    IOrderRepository orderRepo,
    IPaymentGatewayFactory gatewayFactory
) : IPaymentService
{
    public async Task<Result<CreatePaymentIntentResponse>> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken ct)
    {
        var order = await orderRepo.GetByOrderNumberAsync(request.OrderNumber, ct);
        if (order is null)
           return Result<CreatePaymentIntentResponse>.Failure("Order not found", "NOT_FOUND");
        
        if (order.PaymentStatus == Domain.PaymentStatus.Succeeded)
            return Result<CreatePaymentIntentResponse>.Failure("Order already paid", "ALREADY_PAID");
        
        var gateway = gatewayFactory.GetGateway(request.Provider);

        var paymentResult = await gateway.CreatePaymentIntentAsync(order, ct);
        if (!paymentResult.IsSuccess)
            return Result<CreatePaymentIntentResponse>.Failure(paymentResult.ErrorMessage!, paymentResult.ErrorCode!);
        
        order.ExternalPaymentId = paymentResult.Data!.ClientSecret;
        await orderRepo.UpdateAsync(order);

        return Result<CreatePaymentIntentResponse>.Success(
            new CreatePaymentIntentResponse
            {
                ClientSecret = paymentResult.Data.ClientSecret,
                Amount = order.TotalAmount,
                Currency = "nok"
            }
        );
    }
}

