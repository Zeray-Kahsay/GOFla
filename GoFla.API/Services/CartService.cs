using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Cart;
using GoFla.API.Extensions;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class CartService(ICartRepository cartRepository, IRepository<MenuItem> menuItemRepository) : ICartService
{
    public async Task<Result<CartDto>> GetUserCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);
        if (cart is null)
        {
            return Result<CartDto>.Failure("Cart not found", "NOT_FOUND");
        }

        return Result<CartDto>.Success(cart.ToCartDto());
    }


    public async Task<Result<CartDto>> AddItemToCartAsync(string userId, AddToCartDto addToCartDto, CancellationToken cancellationToken = default)
    {
        var cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);
        if (cart is null)
        {
            return Result<CartDto>.Failure("Cart not found", "NOT_FOUND");
        }

        var menuItem = await menuItemRepository.GetByIdAsync(addToCartDto.MenuItemId, cancellationToken);
        if (!menuItem!.IsAvailable)
        {
            return Result<CartDto>.Failure("Menu item is not available", "ITEM_UNAVAILABLE");
        }

        // check if item already exists in cart
        var existingCartItem = cart.Items.FirstOrDefault(i => i.MenuItemId == addToCartDto.MenuItemId);

        // add if not exists, else update quantity
        if (existingCartItem is not null)
        {
            existingCartItem.Quantity += addToCartDto.Quantity;
            existingCartItem.SpecialInstructions = addToCartDto.SpecialInstructions;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                MenuItemId = addToCartDto.MenuItemId,
                Quantity = addToCartDto.Quantity,
                SpecialInstructions = addToCartDto.SpecialInstructions,
                CreatedAt = DateTime.UtcNow
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await cartRepository.UpdateAsync(cart, cancellationToken);

        // Reload cart with all relations
        cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);

        return Result<CartDto>.Success(cart!.ToCartDto());
    }


    public async Task<Result<CartDto>> UpdateItemQuantityAsync(string userId, int cartItemId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
        {
            return Result<CartDto>.Failure("Quantity must be greater than zero", "INVALID_QUANTITY");
        }

        var cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);
        if (cart is null)
        {
            return Result<CartDto>.Failure("Cart not found", "NOT_FOUND");
        }

        var cartItem = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

        if (cartItem is null)
        {
            return Result<CartDto>.Failure("Cart item not found", "NOT_FOUND");
        }

        cartItem.Quantity = quantity;
        cart.UpdatedAt = DateTime.UtcNow;

        await cartRepository.UpdateAsync(cart, cancellationToken);

        // Reload cart with all relations
        cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);

        return Result<CartDto>.Success(cart!.ToCartDto());
    }



    public async Task<Result<CartDto>> RemoveItemFromCartAsync(string userId, int cartItemId, CancellationToken cancellationToken = default)
    {
        var cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);
        if (cart is null)
        {
            return Result<CartDto>.Failure("Cart not found", "NOT_FOUND");
        }

        var cartItem = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

        if (cartItem is null)
        {
            return Result<CartDto>.Failure("Cart item not found", "NOT_FOUND");
        }

        cart.Items.Remove(cartItem);
        cart.UpdatedAt = DateTime.UtcNow;

        await cartRepository.UpdateAsync(cart, cancellationToken);

        // Reload cart with all relations
        cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);

        return Result<CartDto>.Success(cart!.ToCartDto());
    }


    public async Task<Result<bool>> ClearCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);
        if (cart is null)
        {
            return Result<bool>.Failure("Cart not found", "NOT_FOUND");
        }

        cart.Items.Clear();
        cart.UpdatedAt = DateTime.UtcNow;

        await cartRepository.UpdateAsync(cart, cancellationToken);

        return Result<bool>.Success(true);
    }



}
