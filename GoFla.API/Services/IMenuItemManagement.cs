using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.MenuItems;

namespace GoFla.API.Services;

public interface IMenuManagementService
{
    Task<Result<List<MenuItemDto>>> GetAllByRestaurantAsync(int restaurantId);
    Task<Result<MenuItemDto>> CreateAsync(int restaurantId, CreateMenuItemDto dto);
    Task<Result<MenuItemDto>> UpdateAsync(int id, UpdateMenuItemDto dto);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<bool>> ToggleAvailabilityAsync(int id);
}


