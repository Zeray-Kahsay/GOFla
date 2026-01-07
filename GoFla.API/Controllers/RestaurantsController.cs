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
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized("Unauthorized user cannot add restaurant");

        var result = await restaurantService.CreateAsync(dto, userId, cancellationToken);

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


    [HttpPost("{restaurantId:int}/image")]
    public async Task<IActionResult> UploadImage(int restaurantId, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided");
        
        var userId = User.GetUserId();
        if (userId is null)
          return Unauthorized("Unauthorized user");

        var result = await restaurantService.UploadRestaurantImageAsync(restaurantId, userId, file, cancellationToken);

        return result.IsSuccess
                ? Ok(result.Data)
                : Problem(result.ErrorMessage);
    }

    [HttpDelete("{restaurantId:int}/image")]
    public async Task<IActionResult> RemoveImage(int restaurantId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
          return Unauthorized("Unauthorized user");

        var result = await restaurantService.RemoveRestaurantImageAsync(restaurantId, userId, cancellationToken);

        return result.IsSuccess
                ? Ok(result.Data)
                : Problem(result.ErrorMessage);
    }


    [Authorize]
    [HttpGet("my-restaurants")]
    public async Task<IActionResult> GetMyRestaurants(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized("Unauthorized user cannot access restaurants");

        var result = await restaurantService.GetMyRestaurantsAsync(userId, cancellationToken);

        return result.IsSuccess
                ? Ok(result.Data)
                : BadRequest(result.ErrorMessage);
    }

}
