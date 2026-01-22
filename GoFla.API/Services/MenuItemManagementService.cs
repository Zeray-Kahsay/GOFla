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
    ICategoryRepository categoryRepo,
    IUserContext userContext,
    IImageUploadService imageUploadService
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

        // Normalize category name
        var categoryName = dto.CategoryName.Trim();

        if (string.IsNullOrWhiteSpace(categoryName))
            return Result<MenuItemDto>.Failure("Category name is required", "VALIDATION_ERROR");
        // Get or create Category
        var category = await categoryRepo.GetByNameAsync(dto.CategoryName, restaurantId);
        if (category is null)
        {
            category = new Category
            {
                Name = categoryName,
                RestaurantId = restaurantId
            };

            await categoryRepo.AddAsync(category);
        }


        var item = new MenuItem
        {
            RestaurantId = restaurantId,
            CategoryId = category.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = string.Empty,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await menuRepository.AddAsync(item);

        return Result<MenuItemDto>.Success(created.ToMenuItemDto());
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

    public async Task<Result<MenuItemDto>> UploadImageAsync(int menuItemId, IFormFile file)
    {
       if (userContext.UserId is null)
            return Result<MenuItemDto>.Failure("Unauthorized", "UNAUTHORIZED");
        
        var item = await menuRepository.GetByIdAsync(menuItemId);
        if (item is null)
            return Result<MenuItemDto>.Failure("Menu item not found", "NOT_FOUND");
        
        var restaurant = await restaurantRepository.GetByIdAsync(item.RestaurantId);
        if (restaurant is null)
            return Result<MenuItemDto>.Failure("Restaurant not found", "NOT_FOUND");
        
        if (restaurant.OwnerId != userContext.UserId)
            return Result<MenuItemDto>.Failure("Access denied", "FORBIDDEN");
        
        var imageUrl = await imageUploadService.UploadMenuItemImageAsync( menuItemId, file);

        item.ImageUrl = imageUrl;
        item.UpdatedAt = DateTime.UtcNow;

        await menuRepository.UpdateAsync(item);

        return Result<MenuItemDto>.Success(item.ToMenuItemDto());
    }
}

