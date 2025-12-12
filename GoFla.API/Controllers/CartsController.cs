using System;
using GoFla.API.DTOs.Cart;
using GoFla.API.Extensions;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class CartsController (ICartService cartService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetUserCart(CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        var result = await cartService.GetUserCartAsync(userId, cancellationToken);

        return Ok(result);
    }

    [HttpPost("add-item")]
    public async Task<IActionResult> AddItemToCart([FromBody] AddToCartDto dto, CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        var result = await cartService.AddItemToCartAsync(userId, dto, cancellationToken);

        return Ok(result);
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItemQuantity(int cartItemId, [FromBody] UpdateCartItemDto dto, CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        var result = await cartService.UpdateItemQuantityAsync(userId, cartItemId, dto.Quantity, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveItemFromCart(int cartItemId, CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        var result = await cartService.RemoveItemFromCartAsync(userId, cartItemId, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("clear-cart")]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        var result = await cartService.ClearCartAsync(userId, cancellationToken);

        return Ok(result);
    }

    
}
