using System;
using System.Text;
using System.Text.Json;
using GoFla.API.Commons;
using GoFla.API.Data;
using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Repositories;

public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
{
    public FavoriteRepository(AppDbContext _contexte) : base(_contexte) { }


    public async Task<PagedResult<Favorite>> GetUserFavoritesAsync(string userId, string? cursor, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Favorites
            .Include(f => f.Restaurant)
                .ThenInclude(r => r.Address)
            .Where(f => f.UserId == userId);

        if (!string.IsNullOrEmpty(cursor))
        {
            var cursorValue = DecodeCursor<DateTime>(cursor);
            query = query.Where(f => f.CreatedAt > cursorValue);
        }

        query = query.OrderBy(f => f.CreatedAt);

        var items = await query
                .Take(pageSize + 1)
                .ToListAsync(cancellationToken);

        var hasMore = items.Count > pageSize;
        if (hasMore)
        {
            items = items.Take(pageSize).ToList();

        }
        string? nextCursor = null;
        if (hasMore && items.Any())
        {
            nextCursor = EncodeCursor(items.Last().CreatedAt);
        }

        var totalCount = await _context.Favorites
            .Where(f => f.UserId == userId)
            .CountAsync(cancellationToken);

        return new PagedResult<Favorite>
        {
            Items = items,
            TotalCount = totalCount,
            NextCursor = nextCursor,
            HasMore = hasMore
        };

    }


    public async Task<Favorite?> GetByUserAndRestaurantAsync(string userId, int restaurantId, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
            .Include(f => f.Restaurant)
                .ThenInclude(r => r.Address)
            .FirstOrDefaultAsync(f => f.UserId == userId && 
                f.RestaurantId == restaurantId, cancellationToken);
    }

    public async Task<int> GetFavoriteCountAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
            .CountAsync(f => f.RestaurantId == restaurantId, cancellationToken);
    }


    public async Task<bool> IsFavoriteAsync(string userId, int restaurantId, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
           .AnyAsync(f => f.UserId == userId && f.RestaurantId == restaurantId, cancellationToken);
    }

    // Helper methods 
    private static string EncodeCursor<TKey>(TKey value)
    {
        var json = JsonSerializer.Serialize(value);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    private static TKey DecodeCursor<TKey>(string cursor)
    {
        var bytes = Convert.FromBase64String(cursor);
        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<TKey>(json) ?? throw new InvalidOperationException("Failed to deserialize cursor value.");
    }

    public async Task<List<int>> GetFavoriteRestaurantIdsAsync(string userId, List<int> restaurantIds, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId && restaurantIds.Contains(f.RestaurantId))
            .Select(f => f.RestaurantId)
            .ToListAsync(cancellationToken);
    }
}
