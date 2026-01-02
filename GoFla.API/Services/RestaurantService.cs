using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Restaurants;
using GoFla.API.Extensions;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class RestaurantService(IRestaurantRepository restaurantRepository) : IRestaurantService
{
    public async Task<Result<RestaurantDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurantRepository.GetWithDetailsAsync(id, cancellationToken);
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
             cancellationToken: cancellationToken,
             r => r.Address

             );

        var dtoList = pagedResult.Items.Select(r => r.ToRestaurantDto()).ToList();

        return Result<PagedResult<RestaurantDto>>.Success(new PagedResult<RestaurantDto>
        {
            Items = dtoList,
            TotalCount = pagedResult.TotalCount,
            NextCursor = pagedResult.NextCursor,
            HasMore = pagedResult.HasMore
        });
    }


    public async Task<Result<RestaurantDto>> CreateAsync(CreateRestaurantDto dto, string ownerId, CancellationToken cancellationToken = default)
    {
        var restaurant = new Restaurant
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = ownerId,
            Address = new Address
            {
                Label = dto.AddressDto.Label,
                Street = dto.AddressDto.Street,
                City = dto.AddressDto.City,
                State = dto.AddressDto.State,
                PostalCode = dto.AddressDto.PostalCode,
                CountryCode = dto.AddressDto.CountryCode,
                Latitude = dto.AddressDto.Latitude ?? 0,
                Longitude = dto.AddressDto.Longitude ?? 0,
                CreatedAt = DateTime.UtcNow,
                //UserId = ownerId,
            },
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
        var restaurant = await restaurantRepository.GetWithDetailsAsync(id, cancellationToken);
        if (restaurant is null)
        {
            return Result<RestaurantDto>.Failure("Restaurant not found", "NOT_FOUND");
        }

        restaurant.Name = dto.Name;
        restaurant.Description = dto.Description;
        restaurant.Phone = dto.Phone;
        restaurant.DeliveryFee = dto.DeliveryFee;
        restaurant.EstimatedDeliveryTime = dto.EstimatedDeliveryTime;
        restaurant.UpdatedAt = DateTime.UtcNow;

        var address = restaurant.Address;
        address.Label = dto.AddressDto.Label;
        address.Street = dto.AddressDto.Street;
        address.City = dto.AddressDto.City;
        address.State = dto.AddressDto.State;
        address.PostalCode = dto.AddressDto.PostalCode;
        address.CountryCode = dto.AddressDto.CountryCode;
        address.Latitude = dto.AddressDto.Latitude ?? address.Latitude;
        address.Longitude = dto.AddressDto.Longitude ?? address.Longitude;

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
