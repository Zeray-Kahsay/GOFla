using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.MenuItems;

namespace GoFla.API.Services;

public interface IMenuManagementService
{
    Task<Result<PagedResult<MenuItemDto>>> GetAllByRestaurantAsync(
        int restaurantId,
        PaginationParams paginationParams,
        string? search = null,
        int? categoryId = null,
        bool? isAvailable = null,
        CancellationToken cancellationToken = default
    );
    Task<Result<MenuItemDto>> CreateAsync(int restaurantId, CreateMenuItemDto dto);
    Task<Result<MenuItemDto>> UploadImageAsync(int menuItemId, IFormFile file, CancellationToken ct );
    Task<Result<MenuItemDto>> UpdateAsync(int id, UpdateMenuItemDto dto);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<bool>> ToggleAvailabilityAsync(int id);
}


