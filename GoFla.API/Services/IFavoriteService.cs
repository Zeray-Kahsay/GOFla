using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Favorites;

namespace GoFla.API.Services;

public interface IFavoriteService
{
    Task<Result<PagedResult<FavoriteDto>>> GetUserFavoritesAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsFavoriteAsync(string userId, int restaurantId, CancellationToken cancellationToken = default);
    Task<Result<FavoriteDto>> AddFavoriteAsync(string userId, int restaurantId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveFavoriteAsync(string userId, int restaurantId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetFavoriteCountAsync(int restaurantId, CancellationToken cancellationToken = default);
}
