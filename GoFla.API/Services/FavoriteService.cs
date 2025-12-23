using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Favorites;
using GoFla.API.Extensions;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class FavoriteService (IFavoriteRepository favoriteRepository, 
IRestaurantRepository restaurantRepository) : IFavoriteService
{
    public async Task<Result<PagedResult<FavoriteDto>>> GetUserFavoritesAsync(
      string userId,
      PaginationParams paginationParams,
      CancellationToken cancellationToken = default)
    {
        var pagedResult = await favoriteRepository.GetUserFavoritesAsync(
            userId,
            paginationParams.Cursor,
            paginationParams.PageSize,
            cancellationToken
        );

        var dtos = pagedResult.Items.Select(f => f.ToDto()).ToList();

        return Result<PagedResult<FavoriteDto>>.Success(new PagedResult<FavoriteDto>
        {
            Items = dtos,
            TotalCount = pagedResult.TotalCount,
            NextCursor = pagedResult.NextCursor,
            HasMore = pagedResult.HasMore
        });
    }

    public async Task<Result<bool>> IsFavoriteAsync(
        string userId,
        int restaurantId,
        CancellationToken cancellationToken = default)
    {
        var isFavorite = await favoriteRepository.IsFavoriteAsync(userId, restaurantId, cancellationToken);
        return Result<bool>.Success(isFavorite);
    }

    public async Task<Result<FavoriteDto>> AddFavoriteAsync(
        string userId,
        int restaurantId,
        CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant == null)
        {
            return Result<FavoriteDto>.Failure("Restaurant not found", "NOT_FOUND");
        }

        var existing = await favoriteRepository.GetByUserAndRestaurantAsync(userId, restaurantId, cancellationToken);
        if (existing != null)
        {
            return Result<FavoriteDto>.Failure("Restaurant already in favorites", "ALREADY_FAVORITE");
        }

        var favorite = new Favorite
        {
            UserId = userId,
            RestaurantId = restaurantId,
            CreatedAt = DateTime.UtcNow
        };

        await favoriteRepository.AddAsync(favorite, cancellationToken);

        // Reload with restaurant details
        var created = await favoriteRepository.GetByUserAndRestaurantAsync(userId, restaurantId, cancellationToken);
        return Result<FavoriteDto>.Success(created!.ToDto());
    }

    public async Task<Result<bool>> RemoveFavoriteAsync(
        string userId,
        int restaurantId,
        CancellationToken cancellationToken = default)
    {
        var favorite = await favoriteRepository.GetByUserAndRestaurantAsync(userId, restaurantId, cancellationToken);
        if (favorite == null)
        {
            return Result<bool>.Failure("Favorite not found", "NOT_FOUND");
        }

        await favoriteRepository.DeleteAsync(favorite, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<int>> GetFavoriteCountAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        var count = await favoriteRepository.GetFavoriteCountAsync(restaurantId, cancellationToken);
        return Result<int>.Success(count);
    }
}
