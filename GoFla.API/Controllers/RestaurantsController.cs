using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Restaurants;
using GoFla.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class RestaurantsController (IRestaurantService restaurantService) : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRestaurantById(int id, CancellationToken cancellationToken)
    {
        var result = await restaurantService.GetByIdAsync(id, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRestaurants([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var result = await restaurantService.GetAllAsync(paginationParams, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantDto dto, CancellationToken cancellationToken)
    {
        var result = await restaurantService.CreateAsync(dto, cancellationToken);

        return Ok(result);

    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] UpdateRestaurantDto dto, CancellationToken cancellationToken)
    {
        var result = await restaurantService.UpdateAsync(id, dto, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRestaurant(int id, CancellationToken cancellationToken)
    {
        var result = await restaurantService.DeleteAsync(id, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleRestaurantActiveStatus(int id, CancellationToken cancellationToken)
    {
        var result = await restaurantService.ToggleActiveStatusAsync(id, cancellationToken);

        return Ok(result);
    }
    
}
