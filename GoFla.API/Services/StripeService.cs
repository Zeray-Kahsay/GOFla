using System;
using GoFla.API.Commons;
using GoFla.API.Configuration;
using Microsoft.Extensions.Options;
using Stripe;


namespace GoFla.API.Services;

public class StripeService : IStripeService
{
    private readonly StripeSettings _stripeSettings;
    private readonly ILogger<StripeService> _logger;

    public StripeService(IOptions<StripeSettings> stripeSettings, ILogger<StripeService> logger)
    {
        _stripeSettings = stripeSettings.Value;
        _logger = logger;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }
    public Task<Result<bool>> ConfirmPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> CreatePaymentIntentAsync(decimal amount, string userId, string? paymentMehtodId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RefundPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
