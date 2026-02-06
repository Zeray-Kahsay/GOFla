using System;
using GoFla.API.Domain;

namespace GoFla.API.Services;

public class PaymentGatewayFactory (IServiceProvider providerServie) : IPaymentGatewayFactory
{
    public IPaymentGateway GetGateway(PaymentProvider provider)
    {
       return provider switch
       {
           PaymentProvider.Stripe => providerServie.GetRequiredService<StripePaymentGateway>(),
           _ => throw new NotSupportedException("Payment provider not supported")
       };
    }
}
