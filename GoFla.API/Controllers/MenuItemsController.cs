using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.MenuItems;
using GoFla.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class MenuItemsController (IMenuItemService menuItemService) : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMenuItemById(int id, CancellationToken cancellationToken)
    {
        var result = await menuItemService.GetByIdAsync(id, cancellationToken);

        return Ok(result);
    }

    [HttpGet("restaurant/{restaurantId}")]
    public async Task<IActionResult> GetMenuItemsByRestaurantId(int restaurantId, [FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var result = await menuItemService.GetByRestaurantAsync(restaurantId, paginationParams, cancellationToken); 
        return Ok(result);
    }

    [HttpGet("restaurant/{restaurantId}/category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(
        int restaurantId, 
        string category,
         [FromQuery] PaginationParams paginationParams,
          CancellationToken cancellationToken)
    {
        var result = await menuItemService.GetByCategoryAsync(restaurantId, category, paginationParams, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemDto dto, CancellationToken cancellationToken)
    {
        var result = await menuItemService.CreateAsync(dto, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] UpdateMenuItemDto dto, CancellationToken cancellationToken)
    {
        var result = await menuItemService.UpdateAsync(id, dto, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenuItem(int id, CancellationToken cancellationToken)
    {
        var result = await menuItemService.DeleteAsync(id, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPatch("{id}/toggle-availability")]
    public async Task<IActionResult> ToggleMenuItemAvailability(int id, CancellationToken cancellationToken)
    {
        var result = await menuItemService.ToggleAvailabilityAsync(id, cancellationToken);

        return Ok(result);
    }
}
