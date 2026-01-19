using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Cart;
using GoFla.API.Extensions;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class CartsController(ICartService cartService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetUserCart(CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new ApiErrorResponse("UNAUTHORIZED", "User is not authenticated."));
        }
        return HandleResult(await cartService.GetUserCartAsync(userId, cancellationToken));
    }


    [HttpPost("add-item")]
    public async Task<IActionResult> AddItemToCart([FromBody] AddToCartDto dto, CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        return HandleResult(await cartService.AddItemToCartAsync(userId, dto, cancellationToken));

       
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItemQuantity(int cartItemId, [FromBody] UpdateCartItemDto dto, CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        return HandleResult(await cartService.UpdateItemQuantityAsync(userId, cartItemId, dto.Quantity, cancellationToken));

    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveItemFromCart(int cartItemId, CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        return HandleResult(await cartService.RemoveItemFromCartAsync(userId, cartItemId, cancellationToken));

    }

    [HttpDelete("clear-cart")]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        string userId = GetUserId();
        return HandleResult(await cartService.ClearCartAsync(userId, cancellationToken));
   
    }


}
