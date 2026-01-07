using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Restaurants;

namespace GoFla.API.Services;

public interface IRestaurantService
{
    Task<Result<RestaurantDto>> GetByIdAsync(int id, CancellationToken cancellationToken  = default);
    Task<Result<PagedResult<RestaurantDto>>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<Result<RestaurantDto>> CreateAsync(CreateRestaurantDto dto,string ownerId, CancellationToken cancellationToken = default);
    Task<Result<RestaurantDto>> UpdateAsync(int id, UpdateRestaurantDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<bool>> ToggleActiveStatusAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<string>> UploadRestaurantImageAsync(int restaurantId, string ownerId, IFormFile file, CancellationToken cancellationToken);
    Task<Result<bool>> RemoveRestaurantImageAsync(int restaurantId, string ownerId, CancellationToken cancellationToken = default);
    Task<Result<List<RestaurantDto>>> GetMyRestaurantsAsync(string ownerId, CancellationToken cancellationToken = default);  
}
