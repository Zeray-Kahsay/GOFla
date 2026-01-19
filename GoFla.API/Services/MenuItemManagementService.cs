using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.MenuItems;
using GoFla.API.Extensions;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class MenuManagementService(
    IRepository<MenuItem> menuRepository,
    IRestaurantRepository restaurantRepository,
    IRepository<Category> categoryRepository,
    IUserContext userContext
) : IMenuManagementService
{
    public async Task<Result<MenuItemDto>> CreateAsync(
        int restaurantId,
        CreateMenuItemDto dto)
    {
        if (userContext.UserId is null)
            return Result<MenuItemDto>.Failure("Unauthorized", "UNAUTHORIZED");
            
        var restaurant = await restaurantRepository.GetByIdAsync(restaurantId);
        if (restaurant is null)
            return Result<MenuItemDto>.Failure("Restaurant not found", "NOT_FOUND");

        if (restaurant.OwnerId != userContext.UserId)
            return Result<MenuItemDto>.Failure("Access denied", "FORBIDDEN");

        var category = await categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category is null || category.RestaurantId != restaurantId)
            return Result<MenuItemDto>.Failure("Invalid category", "INVALID_CATEGORY");

        var item = new MenuItem
        {
            RestaurantId = restaurantId,
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = string.Empty,
            IsAvailable = true
        };

        await menuRepository.AddAsync(item);

        return Result<MenuItemDto>.Success(item.ToMenuItemDto());
    }

    public async Task<Result<MenuItemDto>> UpdateAsync(
        int menuItemId,
        UpdateMenuItemDto dto)
    {
        var item = await menuRepository.GetByIdAsync(menuItemId);
        if (item is null)
            return Result<MenuItemDto>.Failure("Menu item not found", "NOT_FOUND");

        if (item.Restaurant.OwnerId != userContext.UserId)
            return Result<MenuItemDto>.Failure("Access denied", "FORBIDDEN");

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.Price = dto.Price;
        item.CategoryId = dto.CategoryId;
        item.UpdatedAt = DateTime.UtcNow;

        await menuRepository.UpdateAsync(item);

        return Result<MenuItemDto>.Success(item.ToMenuItemDto());
    }

    public async Task<Result<bool>> DeleteAsync(int menuItemId)
    {
        var item = await menuRepository.GetByIdAsync(menuItemId);
        if (item is null)
            return Result<bool>.Failure("Menu item not found", "NOT_FOUND");
        if (item.Restaurant.OwnerId != userContext.UserId)
            return Result<bool>.Failure("Access denied", "FORBIDDEN");

        await menuRepository.DeleteAsync(item);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ToggleAvailabilityAsync(int menuItemId)
    {
        var item = await menuRepository.GetByIdAsync(menuItemId);
        if (item is null)
            return Result<bool>.Failure("Menu item not found", "NOT_FOUND");

        if (item.Restaurant.OwnerId != userContext.UserId)
            return Result<bool>.Failure("Access denied", "FORBIDDEN");

        item.IsAvailable = !item.IsAvailable;
        item.UpdatedAt = DateTime.UtcNow;

        await menuRepository.UpdateAsync(item);
        return Result<bool>.Success(item.IsAvailable);
    }

    public async Task<Result<List<MenuItemDto>>> GetAllByRestaurantAsync(int restaurantId)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(restaurantId);
        if (restaurant is null)
            return Result<List<MenuItemDto>>.Failure("Restaurant not found", "NOT_FOUND");

        if (restaurant.OwnerId != userContext.UserId)
            return Result<List<MenuItemDto>>.Failure("Access denied", "FORBIDDEN");

        var paged = await menuRepository.GetPagedAsync(
            predicate: mi => mi.RestaurantId == restaurantId,
            orderBy: mi => mi.CreatedAt,
            descending: true,
            cursor: null,
            pageSize: 1000,
            includes: mi => mi.Category
        );

        var dtoItems = paged.Items.Select(mi => mi.ToMenuItemDto()).ToList();

        return Result<List<MenuItemDto>>.Success(dtoItems);
    }
}

