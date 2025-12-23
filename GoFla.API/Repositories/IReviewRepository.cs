using System;
using GoFla.API.Commons;
using GoFla.API.Domain;

namespace GoFla.API.Repositories;

public interface IReviewRepository : IRepository<Review>
{
    Task<PagedResult<Review>> GetRestaurantReviewsAsync(int restaurantId, string? cursor, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<Review>> GetUserReviewsAsync(string userId, string? cursor, int pageSize, CancellationToken cancellationToken = default);
    Task<Review?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HasUserReviewedRestaurantAsync(string userId, int restaurantId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingAsync(int restaurantId, CancellationToken cancellationToken = default);
    Task<Dictionary<int, int>> GetRatingDistributionAsync(int restaurantId, CancellationToken cancellationToken = default);
}
