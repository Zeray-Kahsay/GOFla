using GoFla.API.DTOs.Orders;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class OrdersController (IOrderService orderService) : BaseController
{

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest dto, CancellationToken cancellationToken)
    {
       
        var result = await orderService.CreateOrderFromCartAsync(dto, cancellationToken);

        return HandleResult(result);
    }


    // [HttpGet]
    // public async Task<IActionResult> GetUserOrders([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    // {
    //     

    //     var result = await orderService.GetUserOrderAsync(userId, paginationParams, cancellationToken);

    //     return Ok(result);
    // }

    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetOrderById(int id, CancellationToken cancellationToken)
    // {
    //     string userId = User.GetUserId() ?? string.Empty;
    //     if (string.IsNullOrEmpty(userId))
    //     {
    //         return Unauthorized();
    //     }

    //     var result = await orderService.GetByIdAsync(id,userId, cancellationToken);

    //     return Ok(result);
    // }

    // [HttpGet("number/{orderNumber}")]
    // public async Task<IActionResult> GetOrderByOrderNumber(string orderNumber, CancellationToken cancellationToken)
    // {
    //     string userId = User.GetUserId() ?? string.Empty;
    //     if (string.IsNullOrEmpty(userId))
    //     {
    //         return Unauthorized();
    //     }

    //     var result = await orderService.GetByOrderNumberAsync(orderNumber, userId, cancellationToken);

    //     return Ok(result);
    // }

   

    // [HttpPost("{id}/cancel")]
    // public async Task<IActionResult> CancelOrder(int id, CancellationToken cancellationToken)
    // {
    //     string userId = User.GetUserId() ?? string.Empty;
    //     if (string.IsNullOrEmpty(userId))
    //     {
    //         return Unauthorized();
    //     }

    //     var result = await orderService.CancelOrderAsync(id, userId, cancellationToken);

    //     return Ok(result);
    // }

    // [HttpPatch("{id}/status")]
    // public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto, CancellationToken cancellationToken)
    // {
    //     var result = await orderService.UpdateOrderStatusAsync(id, dto.Status, cancellationToken);

    //     return Ok(result);
    // }



}
