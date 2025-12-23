using System;
using GoFla.API.Commons;
using GoFla.API.Data;
using GoFla.API.Domain;

namespace GoFla.API.Repositories;

public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
{
    public FavoriteRepository(AppDbContext context) : base(context) { }
    public Task<Favorite?> GetByUserAndRestaurantAsync(string userId, int restaurantId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetFavoriteCountAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<Favorite>> GetUserFavoritesAsync(string userId, string? cursor, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsFavoriteAsync(string userId, int restaurantId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
