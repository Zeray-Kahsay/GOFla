using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Cart;

namespace GoFla.API.Services;

public interface ICartService
{
    Task<Result<CartDto>> GetUserCartAsync(CancellationToken cancellationToken = default);
    Task<Result<CartDto>> AddItemToCartAsync(AddToCartDto addToCartDto, CancellationToken cancellationToken = default);
    Task<Result<CartDto>> UpdateItemQuantityAsync(int cartItemId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<CartDto>> RemoveItemFromCartAsync(int cartItemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ClearCartAsync(CancellationToken cancellationToken = default);
}
