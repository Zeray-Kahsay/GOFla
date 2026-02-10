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
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        //Event stripeEvent;

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                 json,
                 Request.Headers["Stripe-signature"],
                 config["Stripe:WebhookSecret"]
             );
            await orderService.HandleStripeWebhookAsync(stripeEvent, ct);

            return Ok();
        }
        catch (StripeException sEx)
        {
            return BadRequest($"Invalid signature: {sEx.Message}");
        }

    }
}
