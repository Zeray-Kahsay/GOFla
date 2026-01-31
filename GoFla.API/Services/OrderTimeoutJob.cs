using System;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class OrderTimeoutJob(IOrderRepository orderRepository, IStripeService stripeService)
{
    public async Task CancelUnpaidOrder(int orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);

        if (order is null || order.PaymentStatus != Domain.PaymentStatus.Pending)
            return;
        
        await stripeService.CancelPaymentIntentAsync(order.PaymentIntentId!);
        order.Status = Domain.OrderStatus.Cancelled;

        await orderRepository.UpdateAsync(order);
    }
}
