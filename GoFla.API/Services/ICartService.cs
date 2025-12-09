using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Cart;

namespace GoFla.API.Services;

public interface ICartService
{
    Task<Result<CartDto>> GetUserCartAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<CartDto>> AddItemToCartAsync(string userId, AddToCartDto addToCartDto, CancellationToken cancellationToken = default);
    Task<Result<CartDto>> UpdateItemQuantityAsync(string userId, int cartItemId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<CartDto>> RemoveItemFromCartAsync(string userId, int cartItemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ClearCartAsync(string userId, CancellationToken cancellationToken = default);
}
