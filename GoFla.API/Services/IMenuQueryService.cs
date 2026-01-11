using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.MenuItems;

namespace GoFla.API.Services;


public interface IMenuQueryService
{
    Task<Result<MenuItemDto>> GetByIdAsync(int id);

    Task<Result<PagedResult<MenuItemDto>>> GetByRestaurantAsync(
        int restaurantId,
        PaginationParams paginationParams);

    Task<Result<PagedResult<MenuItemDto>>> GetByCategoryAsync(
        int restaurantId,
        int categoryId,
        PaginationParams paginationParams);
}

// Task<Result<MenuItemDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
// Task<Result<PagedResult<MenuItemDto>>> GetByRestaurantAsync(int restaurantId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
// Task<Result<PagedResult<MenuItemDto>>> GetByCategoryAsync(int restaurantId, string category, PaginationParams paginationParams, CancellationToken cancellationToken = default);
// Task<Result<MenuItemDto>> CreateAsync(CreateMenuItemDto dto, CancellationToken cancellationToken = default);
// Task<Result<MenuItemDto>> UpdateAsync(int id, UpdateMenuItemDto dto, CancellationToken cancellationToken = default);
// Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
// Task<Result<bool>> ToggleAvailabilityAsync(int id, CancellationToken cancellationToken = default);

