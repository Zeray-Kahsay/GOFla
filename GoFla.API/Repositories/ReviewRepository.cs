using System;
using GoFla.API.Commons;
using GoFla.API.Data;
using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Repositories;

public class ReviewRepository(AppDbContext context) : Repository<Review>(context), IReviewRepository
{
    public async Task<PagedResult<Review>> GetRestaurantReviewsAsync(int restaurantId, string? cursor, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(
            predicate: r => r.RestaurantId == restaurantId && r.IsApproved && !r.IsFlagged,
            orderBy: r => r.CreatedAt,
            cursor: cursor,
            pageSize: pageSize,
            cancellationToken: cancellationToken
        );
    }


    public async Task<PagedResult<Review>> GetUserReviewsAsync(string userId, string? cursor, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(
            predicate: r => r.UserId == userId,
            orderBy: r => r.CreatedAt,
            cursor: cursor,
            pageSize: pageSize,
            cancellationToken: cancellationToken
        );
    }


    public async Task<Review?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
       return await context.Reviews
            .Include(r => r.User)
            .Include(r => r.Restaurant)
            .Include(r => r.Responses)
               .ThenInclude(rr => rr.Responder)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

        

    public async Task<bool> HasUserReviewedRestaurantAsync(string userId, int restaurantId, CancellationToken cancellationToken = default)
    {
       return await context.Reviews
            .AnyAsync(r => r.UserId == userId && r.RestaurantId == restaurantId, cancellationToken);
    }


    public async Task<double> GetAverageRatingAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        var reviews = await context.Reviews
                  .Where(r => r.RestaurantId == restaurantId && r.IsApproved && !r.IsFlagged)
                  .ToListAsync(cancellationToken);

        return reviews.Count != 0 ? reviews.Average(r => r.Rating) : 0;
    }

    public async Task<Dictionary<int, int>> GetRatingDistributionAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        var distribution = await context.Reviews
                .Where(r => r.RestaurantId == restaurantId && r.IsApproved && !r.IsFlagged)
                .GroupBy(r => r.Rating)
                .Select(g => new {Rating = g.Key, Count = g.Count()})
                .ToListAsync(cancellationToken);
        
        return distribution.ToDictionary(d => d.Rating, d => d.Count);
    }



}
