using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.MenuItems;
using GoFla.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GoFla.API.Controllers;

public class MenuItemsController(
    IMenuQueryService menuQueryService,
    IMenuManagementService menuManagementService) : BaseController
{
    // CUSTOM ENDPOINTS 

    //Get menu items for a restaurant - customer view
    [HttpGet("restaurants/{restaurantId}/menu-items")]
    public async Task<IActionResult> GetByRestaurant(int restaurantId, [FromQuery] PaginationParams paginationParams)
    {
        return HandleResult(await menuQueryService.GetByRestaurantAsync(restaurantId, paginationParams));

    }

    // Get menu item by id - customer view
    [HttpGet("menu-items/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return HandleResult(await menuQueryService.GetByIdAsync(id));

    }

    //OWNER ENDPOINTS

    // Get all menu items for owner dashboard
    [Authorize]
    [HttpGet("owner/restaurants/{restaurantId:int}/menu-items")]
    public async Task<IActionResult> GetByRestaurantForOwner(int restaurantId, [FromQuery] PaginationParams paginationParams) // Good if return type is PaginatedList<MenuItemDto>
    {
        return HandleResult(await menuManagementService.GetAllByRestaurantAsync(restaurantId));

    }

    // Create a new menu item

    [Authorize]
    [HttpPost("owner/{restaurantId}/menu-items")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create(int restaurantId, [FromForm] CreateMenuItemDto createMenuItemDto)
    {
        return HandleResult(await menuManagementService.CreateAsync(restaurantId, createMenuItemDto));

    }

    // Update an existing menu item
    [Authorize]
    [HttpPut("owner/menu-items/{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuItemDto updateMenuItemDto)
    {
        return HandleResult(await menuManagementService.UpdateAsync(id, updateMenuItemDto));

    }

    // Delete a menu item
    [Authorize]
    [HttpDelete("owner/menu-items/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return HandleResult(await menuManagementService.DeleteAsync(id));

    }


    // Toggle availability of a menu item
    [Authorize]
    [HttpPatch("owner/menu-items/{id:int}/toggle-availability")]
    public async Task<IActionResult> ToggleAvailability(int id)
    {
        return HandleResult(await menuManagementService.ToggleAvailabilityAsync(id));

    }

}
