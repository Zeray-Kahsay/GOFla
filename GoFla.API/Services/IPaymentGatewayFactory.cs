using System;
using GoFla.API.Domain;

namespace GoFla.API.Services;

public interface IPaymentGatewayFactory
{
    IPaymentGateway GetGateway(PaymentProvider provider);
}
