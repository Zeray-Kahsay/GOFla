using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Restaurants;
using GoFla.API.Extensions;
using GoFla.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class RestaurantsController(IRestaurantService restaurantService) : BaseController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRestaurantById(int id, CancellationToken cancellationToken)
    {
        return HandleResult(await restaurantService.GetByIdAsync(id, cancellationToken));
    
    }


    [HttpGet]
    public async Task<IActionResult> GetAllRestaurants([FromQuery] PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        return HandleResult(await restaurantService.GetAllAsync(paginationParams, cancellationToken));

    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantDto dto, CancellationToken cancellationToken)
    {
       string userId = GetUserId();

        return HandleResult(await restaurantService.CreateAsync(dto, userId, cancellationToken));

    }


    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] UpdateRestaurantDto dto, CancellationToken cancellationToken)
    {
        return HandleResult(await restaurantService.UpdateAsync(id, dto, cancellationToken));

    }


    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRestaurant(int id, CancellationToken cancellationToken)
    {
        return HandleResult(await restaurantService.DeleteAsync(id, cancellationToken));

    }


    [Authorize]
    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleRestaurantActiveStatus(int id, CancellationToken cancellationToken)
    {
        return HandleResult(await restaurantService.ToggleActiveStatusAsync(id, cancellationToken));
        
    }


    [HttpPost("{restaurantId:int}/image")]
    public async Task<IActionResult> UploadImage(int restaurantId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided");
        
        string userId = GetUserId();

        return HandleResult(await restaurantService.UploadRestaurantImageAsync(restaurantId, userId, file, cancellationToken));

    }

    [HttpDelete("{restaurantId:int}/image")]
    public async Task<IActionResult> RemoveImage(int restaurantId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        return HandleResult(await restaurantService.RemoveRestaurantImageAsync(restaurantId, userId, cancellationToken));
      
    }


    [Authorize]
    [HttpGet("my-restaurants")]
    public async Task<IActionResult> GetMyRestaurants(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
       

        return HandleResult(await restaurantService.GetMyRestaurantsAsync(userId, cancellationToken));

    }

}
