using GoFla.API.DTOs.Cart;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class CartsController(ICartService cartService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetUserCart(CancellationToken cancellationToken)
    {
    
        return HandleResult(await cartService.GetUserCartAsync(cancellationToken));
    }


    [HttpPost("add-item")]
    public async Task<IActionResult> AddItemToCart([FromBody] AddToCartDto dto, CancellationToken cancellationToken)
    {
       
        return HandleResult(await cartService.AddItemToCartAsync(dto, cancellationToken));
       
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateCartItemQuantity(int cartItemId, [FromBody] UpdateCartItemDto dto, CancellationToken cancellationToken)
    {
        return HandleResult(await cartService.UpdateItemQuantityAsync(cartItemId, dto.Quantity, cancellationToken));

    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveItemFromCart(int cartItemId, CancellationToken cancellationToken)
    {
        return HandleResult(await cartService.RemoveItemFromCartAsync(cartItemId, cancellationToken));

    }

    [HttpDelete("clear-cart")]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        return HandleResult(await cartService.ClearCartAsync(cancellationToken));
   
    }

}
