using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Cart;
using GoFla.API.Extensions;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class CartService(ICartRepository cartRepository, IUserContext userContext, IRepository<MenuItem> menuItemRepository) : ICartService
{
    public async Task<Result<CartDto>> GetUserCartAsync(CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;
        if (userId is null)
            return Result<CartDto>.Failure("Unauthorized", "UNAUTHORIZED");

        var cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);
        if (cart is null)
        {
            return Result<CartDto>.Failure("Cart not found", "NOT_FOUND");
        }

        return Result<CartDto>.Success(cart.ToCartDto());
    }


    public async Task<Result<CartDto>> AddItemToCartAsync(AddToCartDto addToCartDto, CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;
        if (userId is null)
            return Result<CartDto>.Failure("Unauthorized", "UNAUTHORIZED");
        
        if (addToCartDto.Quantity <= 0)
            return Result<CartDto>.Failure("Invalid quantity", "INVALID_QUANTITY");

        var menuItem = await menuItemRepository.GetByIdAsync(addToCartDto.MenuItemId, cancellationToken);
        if (!menuItem!.IsAvailable)
        {
            return Result<CartDto>.Failure("Menu item is not available", "ITEM_UNAVAILABLE");
        }

        // Get or create cart 
        var cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);
        if (cart is null)
        {
            cart = new Cart
            {
                UserId = userId,
                RestaurantId = menuItem.RestaurantId,
                CreatedAt = DateTime.UtcNow
            };

            await cartRepository.AddAsync(cart, cancellationToken);
        }

        // Single Restaurant Rule
        if (cart.Items.Any() && cart.RestaurantId != menuItem.RestaurantId)
        {
            return Result<CartDto>.Failure(
                "You can only order from one restaurant at a time",
                "MULTIPLE_RESTAURANT_NOT_ALLOWED"
            );
        }


        // check if item already exists in cart
        var existingCartItem = cart.Items.FirstOrDefault(i => i.MenuItemId == addToCartDto.MenuItemId);

        // add if not exists, else update quantity
        if (existingCartItem is not null)
        {
            existingCartItem.Quantity += addToCartDto.Quantity;
            existingCartItem.SpecialInstructions = addToCartDto.SpecialInstructions;
            existingCartItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                MenuItemId = menuItem.Id,
                // SNAPSHOT DATA
                Name = menuItem.Name,
                ImageUrl = menuItem.ImageUrl,
                UnitPrice = menuItem.Price,

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


    public async Task<Result<CartDto>> UpdateItemQuantityAsync(int cartItemId, int quantity, CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;
        if (userId is null)
            return Result<CartDto>.Failure("Unauthorized", "UNAUTHORIZED");

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



    public async Task<Result<CartDto>> RemoveItemFromCartAsync(int cartItemId, CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;
        if (userId is null)
            return Result<CartDto>.Failure("Unauthorized", "UNAUTHORIZED");
            
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


    public async Task<Result<bool>> ClearCartAsync(CancellationToken cancellationToken = default)
    {
        var userId = userContext.UserId;
        if (userId is null)
            return Result<bool>.Failure("Unauthorized", "UNAUTHORIZED");

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
