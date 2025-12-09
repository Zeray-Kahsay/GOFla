using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Restaurants;
using GoFla.API.Extensions;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class RestaurantService (IRestaurantRepository restaurantRepository) : IRestaurantService
{
    public async Task<Result<RestaurantDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(id, cancellationToken);
        if (restaurant is null)
        {
            return Result<RestaurantDto>.Failure("Restaurant not found", "NOT_FOUND");
        }

        return Result<RestaurantDto>.Success(restaurant.ToRestaurantDto());
    }


    public async Task<Result<PagedResult<RestaurantDto>>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
       var pagedResult = await restaurantRepository.GetPagedAsync(
            predicate: r => r.IsActive,
            orderBy: r => r.Id,
            cursor: paginationParams.Cursor,
            pageSize: paginationParams.PageSize,
            cancellationToken: cancellationToken);
        
        var dtoList = pagedResult.Items.Select(r => r.ToRestaurantDto()).ToList();

        return Result<PagedResult<RestaurantDto>>.Success(new PagedResult<RestaurantDto>
        {
            Items = dtoList,
            TotalCount = pagedResult.TotalCount,
            NextCursor = pagedResult.NextCursor,
            HasMore = pagedResult.HasMore
        });
    }


    public async Task<Result<RestaurantDto>> CreateAsync(CreateRestaurantDto dto, CancellationToken cancellationToken = default)
    {
       var restaurant = new Restaurant
       {
           Name = dto.Name,
           Description = dto.Description,
           Address = dto.Address,
           Phone = dto.Phone,
           DeliveryFee = dto.DeliveryFee,
           EstimatedDeliveryTime = dto.EstimatedDeliveryTime,
           ImageUrl = string.Empty, // Will be updated via separate endpoint
           IsActive = true,
           CreatedAt = DateTime.UtcNow
       };

       var created = await restaurantRepository.AddAsync(restaurant, cancellationToken);
       
       return Result<RestaurantDto>.Success(created.ToRestaurantDto());
    }
    
    public async Task<Result<RestaurantDto>> UpdateAsync(int id, UpdateRestaurantDto dto, CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(id, cancellationToken);
        if (restaurant is null)
        {
            return Result<RestaurantDto>.Failure("Restaurant not found", "NOT_FOUND");
        }

        restaurant.Name = dto.Name;
        restaurant.Description = dto.Description;
        restaurant.Address = dto.Address;
        restaurant.Phone = dto.Phone;
        restaurant.DeliveryFee = dto.DeliveryFee;
        restaurant.EstimatedDeliveryTime = dto.EstimatedDeliveryTime;
        restaurant.UpdatedAt = DateTime.UtcNow;

        await restaurantRepository.UpdateAsync(restaurant, cancellationToken);

        return Result<RestaurantDto>.Success(restaurant.ToRestaurantDto());

    }


    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(id, cancellationToken);
        if (restaurant is null)
        {
            return Result<bool>.Failure("Restaurant not found", "NOT_FOUND");
        }

        await restaurantRepository.DeleteAsync(restaurant, cancellationToken);
        return Result<bool>.Success(true);
    }



    public async Task<Result<bool>> ToggleActiveStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(id, cancellationToken);
        if (restaurant is null)
        {
            return Result<bool>.Failure("Restaurant not found", "NOT_FOUND");
        }

        restaurant.IsActive = !restaurant.IsActive;
        restaurant.UpdatedAt = DateTime.UtcNow;

        await restaurantRepository.UpdateAsync(restaurant, cancellationToken);
        return Result<bool>.Success(true);
    }

}
