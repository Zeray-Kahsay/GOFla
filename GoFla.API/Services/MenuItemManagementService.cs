using Microsoft.EntityFrameworkCore;
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

        // Ownership check
        var restaurant = await restaurantRepository.GetByIdAsync(item.RestaurantId);
        if (restaurant is null)
            return Result<MenuItemDto>.Failure("Restaurant not found", "NOT_FOUND");

        if (restaurant.OwnerId != userContext.UserId)
            return Result<MenuItemDto>.Failure("Access denied", "FORBIDDEN");

        var category = await categoryRepo.GetByIdAsync(dto.CategoryId);
        if (category is null || category.RestaurantId != item.RestaurantId)
            return Result<MenuItemDto>.Failure("Invalid category", "INVALID_CATEGORY");

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

        // Ownership check
        var restaurant = await restaurantRepository.GetByIdAsync(item.RestaurantId);
        if (restaurant is null)
            return Result<bool>.Failure("Restaurant not found", "NOT_FOUND");

        if (restaurant.OwnerId != userContext.UserId)
            return Result<bool>.Failure("Access denied", "FORBIDDEN");

        await menuRepository.DeleteAsync(item);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ToggleAvailabilityAsync(int menuItemId)
    {
        var item = await menuRepository.GetByIdAsync(menuItemId);
        if (item is null)
            return Result<bool>.Failure("Menu item not found", "NOT_FOUND");

        // Ownership check
        var restaurant = await restaurantRepository.GetByIdAsync(item.RestaurantId);
        if (restaurant is null)
            return Result<bool>.Failure("Restaurant not found", "NOT_FOUND");

        if (restaurant.OwnerId != userContext.UserId)
            return Result<bool>.Failure("Access denied", "FORBIDDEN");

        item.IsAvailable = !item.IsAvailable;
        item.UpdatedAt = DateTime.UtcNow;

        await menuRepository.UpdateAsync(item);
        return Result<bool>.Success(item.IsAvailable);
    }

    public async Task<Result<PagedResult<MenuItemDto>>> GetAllByRestaurantAsync(
        int restaurantId,
        PaginationParams paginationParams,
        string? search = null,
        int? categoryId = null,
        bool? isAvailable = null,
        CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(restaurantId);
        if (restaurant is null)
            return Result<PagedResult<MenuItemDto>>.Failure("Restaurant not found", "NOT_FOUND");

        if (userContext.UserId is null)
            return Result<PagedResult<MenuItemDto>>.Failure("Unauthorized", "UNAUTHORIZED");

        if (restaurant.OwnerId != userContext.UserId)
            return Result<PagedResult<MenuItemDto>>.Failure("Access denied", "FORBIDDEN");

        // Normalize search
        search = string.IsNullOrWhiteSpace(search) ? null : search.Trim().ToLower();

        // Enforce limits
        paginationParams = paginationParams with
        {
            PageSize = Math.Clamp(paginationParams.PageSize, 1, 50)
        };


        var paged = await menuRepository.GetPagedAsync(
            predicate: mi =>
            mi.RestaurantId == restaurantId
            && (isAvailable == null || mi.IsAvailable == isAvailable.Value)
            && (categoryId == null || mi.CategoryId == categoryId.Value)
            && (search == null
                || EF.Functions.Like(mi.Name, $"%{search}")
                || EF.Functions.Like(mi.Description, $"%{search}")), // mi.Description.ToLower().Contains(search)
            orderBy: mi => mi.CreatedAt,
            descending: true,
            cursor: paginationParams.Cursor,
            pageSize: paginationParams.PageSize,
            includes: mi => mi.Category
        );

        var dtoItems = paged.Items.Select(mi => mi.ToMenuItemDto()).ToList();

        return Result<PagedResult<MenuItemDto>>.Success(new PagedResult<MenuItemDto>
        {
            Items = dtoItems,
            TotalCount = paged.TotalCount,
            HasMore = paged.HasMore,
            NextCursor = paged.NextCursor
        });
    }

    public async Task<Result<MenuItemDto>> UploadImageAsync(int menuItemId, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return Result<MenuItemDto>.Failure("Image file is required","INVALID_IMAGE");
        
        if (!file.ContentType.Contains("image"))
            return Result<MenuItemDto>.Failure("Invalid image type", "INVALID_IMAGE");

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

        var imageUrl = await imageUploadService.UploadMenuItemImageAsync(menuItemId, file, ct);

        item.ImageUrl = imageUrl;
        item.UpdatedAt = DateTime.UtcNow;

        await menuRepository.UpdateAsync(item);

        return Result<MenuItemDto>.Success(item.ToMenuItemDto());
    }
}

