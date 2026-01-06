using System;
using GoFla.API.Commons;
using GoFla.API.Domain;

namespace GoFla.API.Repositories;

public interface IFavoriteRepository : IRepository<Favorite>
{
    Task<PagedResult<Favorite>> GetUserFavoritesAsync(string userId, string? cursor, int pageSize, CancellationToken cancellationToken = default);
    Task<Favorite?> GetByUserAndRestaurantAsync(string userId, int restaurantId, CancellationToken cancellationToken = default);
    Task<bool> IsFavoriteAsync(string userId, int restaurantId, CancellationToken cancellationToken = default);
    Task<int> GetFavoriteCountAsync(int restaurantId, CancellationToken cancellationToken = default);
    Task<List<int>> GetFavoriteRestaurantIdsAsync(string userId, List<int> restaurantIds, CancellationToken cancellationToken = default);
}
