using System;
using GoFla.API.DTOs.Payment;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class PaymentsController (IPaymentService paymentService) : BaseController
{
    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent( [FromBody] CreatePaymentIntentRequest request, CancellationToken ct)
    {
        var result = await paymentService.CreatePaymentIntentAsync(request, ct);
       return HandleResult(result);
    }
}
