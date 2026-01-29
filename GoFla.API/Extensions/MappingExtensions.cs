using System;
using GoFla.API.Domain;
using GoFla.API.DTOs.Address;
using GoFla.API.DTOs.Auth;
using GoFla.API.DTOs.Cart;
using GoFla.API.DTOs.Category;
using GoFla.API.DTOs.Favorites;
using GoFla.API.DTOs.MenuItems;
using GoFla.API.DTOs.Orders;
using GoFla.API.DTOs.Restaurants;
using GoFla.API.DTOs.Review;

namespace GoFla.API.Extensions;

public static class MappingExtensions
{
    // User mappings 
    public static UserDto ToUserDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfileImageUrl = user.ProfileImageUrl
        };
    }

    // Restaurant mappings 
    public static RestaurantDto ToRestaurantDto(this Restaurant restaurant, HashSet<int>? favoriteIds = null)
    {
        return new RestaurantDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            Description = restaurant.Description,
            ImageUrl = restaurant.ImageUrl,
            Phone = restaurant.Phone,
            DeliveryFee = restaurant.DeliveryFee,
            EstimatedDeliveryTime = restaurant.EstimatedDeliveryTime,
            IsActive = restaurant.IsActive,
            IsFavorite = favoriteIds?.Contains(restaurant.Id) ?? false,
            AddressDto = restaurant.Address is null
            ? null!
            : restaurant.Address.ToAddressDto()
        };
    }

    // MenuItem mappings 
    public static MenuItemDto ToMenuItemDto(this MenuItem menuItem)
    {
        return new MenuItemDto
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Description = menuItem.Description,
            Price = menuItem.Price,
            ImageUrl = menuItem.ImageUrl,
            CategoryId = menuItem.CategoryId,
            CategoryName = menuItem.Category?.Name ?? string.Empty,
            IsAvailable = menuItem.IsAvailable,
            RestaurantId = menuItem.RestaurantId,
        };
    }

    // Cart mappings 
    public static CartDto ToCartDto(this Cart cart)
    {
        var items = cart.Items.Select(ci => ci.ToCartItemDto()).ToList();
        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            SubTotal = items.Sum(i => i.ItemTotal),
            TotalItems = items.Sum(i => i.Quantity)
        };
    }
    public static CartItemDto ToCartItemDto(this CartItem cartItem)
    {
        return new CartItemDto
        {
            Id = cartItem.Id,
            MenuItemId = cartItem.MenuItemId,
            Name = cartItem.MenuItem.Name,
            ImageUrl = cartItem.MenuItem.ImageUrl ?? string.Empty,
            Price = cartItem.MenuItem.Price,
            Quantity = cartItem.Quantity,
            SpecialInstructions = cartItem.SpecialInstructions,
            ItemTotal = cartItem.MenuItem.Price * cartItem.Quantity,
            RestaurantName = cartItem.MenuItem.Restaurant?.Name ?? string.Empty
        };
    }

    // Order mappings 
    public static OrderDto ToOrderDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            RestaurantName = order.Restaurant?.Name ?? string.Empty,
            Status = order.Status.ToString(),
            SubTotal = order.SubTotal,
            DeliveryFee = order.DeliveryFee,
            Tax = order.Tax,
            TotalAmount = order.TotalAmount,
            PaymentStatus = order.PaymentStatus.ToString(),
            //DeliveryAddress = order.DeliveryAddress.ToAddressDto(),
            Items = order.Items.Select(i => i.ToOrderItemDto()).ToList(),
            CreatedAt = order.CreatedAt
        };
    }

    public static OrderItemDto ToOrderItemDto(this OrderItem orderItem)
    {
        return new OrderItemDto
        {
            Id = orderItem.MenuItemId,
           // Name = orderItem.MenuItem?.Name ?? string.Empty,
            Quantity = orderItem.Quantity,
            //Price = orderItem.Price,
            SpecialInstructions = orderItem.SpecialInstructions
        };
    }

    // Address mappings
    public static AddressDto ToAddressDto(this Address address)
    {
        return new AddressDto
        {
            Id = address.Id,
            Label = address.Label,
            Street = address.Street,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            CountryCode = address.CountryCode,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            IsDefault = address.IsDefault
        };
    }

    // Review mappings
    public static ReviewDto ToReviewDto(this Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            UserId = review.UserId,
            UserName = $"{review.User.FirstName} {review.User.LastName}",
            UserProfileImage = review.User.ProfileImageUrl,
            RestaurantId = review.RestaurantId,
            RestaurantName = review.Restaurant?.Name ?? string.Empty,
            Rating = review.Rating,
            Title = review.Title,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            Responses = review.Responses.Select(r => r.ToReviewResponseDto()).ToList()
        };
    }

    public static ReviewResponseDto ToReviewResponseDto(this ReviewResponse response)
    {
        return new ReviewResponseDto
        {
            Id = response.Id,
            ResponderName = $"{response.Responder.FirstName} {response.Responder.LastName}",
            ResponseText = response.ResponseText,
            CreatedAt = response.CreatedAt
        };
    }

    // Favorite mappings
    public static FavoriteDto ToFavoriteDto(this Favorite favorite)
    {
        return new FavoriteDto
        {
            Id = favorite.Id,
            RestaurantId = favorite.RestaurantId,
            RestaurantName = favorite.Restaurant.Name ?? string.Empty,
            RestaurantImage = favorite.Restaurant.ImageUrl ?? string.Empty,
            RestaurantAddress = favorite.Restaurant.Address.Street ?? string.Empty,
            //DeliveryFee = favorite.Restaurant.DeliveryFee,
            CreatedAt = favorite.CreatedAt
        };
    }

    //Category mappings
    public static CategoryDto ToCategoryDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            SortOrder = category.SortOrder
        };
    }

    // 
    // public static int MapErrorCodeToStatusCode(this string errorCode) => errorCode switch
    // {
    //     "NOT_FOUND" => 404,
    //     "FORBIDDEN" => 403,
    //     "INVALID_CATEGORY" => 400,
    //     _ => 500
    // };
}
