using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Review;

namespace GoFla.API.Services;

public interface IReviewService
{
    Task<Result<ReviewDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ReviewDto>>> GetRestaurantReviewsAsync(int restaurantId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ReviewDto>>> GetUserReviewsAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<Result<RestaurantRatingDto>> GetRestaurantRatingAsync(int restaurantId, CancellationToken cancellationToken = default);
    Task<Result<ReviewDto>> CreateAsync(string userId, CreateReviewDto dto, CancellationToken cancellationToken = default);
    Task<Result<ReviewDto>> UpdateAsync(int id, string userId, UpdateReviewDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<Result<ReviewDto>> AddResponseAsync(int reviewId, string responderId, CreateReviewResponseDto dto, CancellationToken cancellationToken = default);
}
