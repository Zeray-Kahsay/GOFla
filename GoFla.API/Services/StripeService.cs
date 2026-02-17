using System;
using GoFla.API.Commons;
using GoFla.API.Configuration;
using GoFla.API.DTOs.Payment;
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

    public async Task<Result<StripeIntentResult>> CreatePaymentIntentAsync(decimal amount, string currency, string orderNumber, CancellationToken ct)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    {"orderNumber", orderNumber}
                }

            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options, cancellationToken: ct);

            return Result<StripeIntentResult>.Success(new StripeIntentResult(intent.Id, intent.ClientSecret));
            
            
        }
        catch (Exception ex)
        {
            return Result<StripeIntentResult>.Failure("Stripe error: " + ex.Message, "STRIPE_ERROR" );
        }
        
    }



    public async Task<Result<bool>> ConfirmPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.ConfirmAsync(paymentIntentId, cancellationToken: cancellationToken);

            if (paymentIntent.Status == "succeeded")
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure("Payment confirmation failed", "PAYMENT_CONFIRMATION_FAILED");
        }
        catch (StripeException ex)
        {
            
            _logger.LogError(ex, "Stripe payment confirmation failed");
            return Result<bool>.Failure(ex.Message, "PAYMENT_CONFIRMATION_FAILED");
        }
    }


    public async Task<Result<bool>> RefundPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
            };

            var service = new RefundService();
            var refund = await service.CreateAsync(options, cancellationToken: cancellationToken);

            if (refund.Status == "succeeded" || refund.Status == "pending")
            {
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure("Payment refund failed", "PAYMENT_REFUND_FAILED");
        }
        catch (StripeException ex)
        {
            
            _logger.LogError(ex, "Stripe payment refund failed");
            return Result<bool>.Failure(ex.Message, "PAYMENT_REFUND_FAILED");
        }
        
    }

    public async Task<Result<string>> CreateCustomerAsync(string email, string name, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new CustomerCreateOptions
            {
                Email = email,
                Name = name,
                Metadata = new Dictionary<string, string>
                {
                    { "userId",  userId },
                }
            };

            var service = new CustomerService();
            var customer = await service.CreateAsync(options, cancellationToken: cancellationToken);

            return Result<string>.Success(customer.Id);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe customer creation failed");
            return Result<string>.Failure(ex.Message, "CUSTOMER_CREATION_FAILED");
        }
    }

    public async Task<Result<List<PaymentMethodDto>>> GetCustomerPaymentMethodsAsync(string customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card",
            };

            var service = new PaymentMethodService();
            var paymentMethods = await service.ListAsync(options, cancellationToken: cancellationToken);

            var dtos = paymentMethods.Data.Select(pm => new PaymentMethodDto
            {
                Id = pm.Id,
                Brand = pm.Card.Brand,
                Last4 = pm.Card.Last4,
                ExpiryMonth = pm.Card.ExpMonth,
                ExpiryYear = pm.Card.ExpYear
            }).ToList();

            return Result<List<PaymentMethodDto>>.Success(dtos);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe fetching customer payment methods failed");
            return Result<List<PaymentMethodDto>>.Failure(ex.Message, "FETCH_PAYMENT_METHODS_FAILED");
        }
       
    }

    public async Task<Result<bool>> AttachPaymentMethodToCustomerAsync(string paymentMethodId, string customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new PaymentMethodAttachOptions
            {
                Customer = customerId,
            };

            var service = new PaymentMethodService();
            await service.AttachAsync(paymentMethodId, options, cancellationToken: cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe attaching payment method to customer failed");
            return Result<bool>.Failure(ex.Message, "ATTACH_PAYMENT_METHOD_FAILED");
        }
    }

    public async Task<Result<bool>> DetachPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new PaymentMethodService();
            await service.DetachAsync(paymentMethodId, cancellationToken: cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe detaching payment method failed");
            return Result<bool>.Failure(ex.Message, "DETACH_PAYMENT_METHOD_FAILED");
        }
    }

    public async Task<Result<bool>> CancelPaymentIntentAsync(string paymentIntentId, CancellationToken ct = default)
    {
        try
        {
            var service = new PaymentIntentService();

            await service.CancelAsync(paymentIntentId, null);

            return Result<bool>.Success(true);
        }
        catch (StripeException ex)
        {
            return Result<bool>.Failure(ex.Message, "STRIPE_ERROR");
        }
    }
}
