using System;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace GoFla.API.Controllers;

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController(IOrderService orderService, IConfiguration config) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> HandleWebhook(CancellationToken ct)
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        var stripeSignature = Request.Headers["Stripe-signature"];

        var endpointSecret = config["Stripe:WebhookSecret"];

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json, stripeSignature, endpointSecret
            );
        }
        catch(Exception e)
        {
            return BadRequest($"Invalid signature: {e.Message}");
        }

        await orderService.HandleStripeWebhookAsync(stripeEvent, ct);

        return Ok();
    }
}
