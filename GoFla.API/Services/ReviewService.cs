using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Review;
using GoFla.API.Extensions;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class ReviewService (
    IReviewRepository reviewRepository,
     IRestaurantRepository restaurantRepository, 
     IOrderRepository orderRepository) : IReviewService
{
    public async Task<Result<ReviewDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetWithDetailsAsync(id, cancellationToken);
        if (review == null)
        {
            return Result<ReviewDto>.Failure("Review not found", "NOT_FOUND");
        }

        return Result<ReviewDto>.Success(review.ToDto());
    }

    public async Task<Result<PagedResult<ReviewDto>>> GetRestaurantReviewsAsync(
        int restaurantId,
        PaginationParams paginationParams,
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await reviewRepository.GetRestaurantReviewsAsync(
            restaurantId,
            paginationParams.Cursor,
            paginationParams.PageSize,
            cancellationToken
        );

        var dtos = pagedResult.Items.Select(r => r.ToDto()).ToList();

        return Result<PagedResult<ReviewDto>>.Success(new PagedResult<ReviewDto>
        {
            Items = dtos,
            TotalCount = pagedResult.TotalCount,
            NextCursor = pagedResult.NextCursor,
            HasMore = pagedResult.HasMore
        });
    }

    public async Task<Result<PagedResult<ReviewDto>>> GetUserReviewsAsync(
        string userId,
        PaginationParams paginationParams,
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await reviewRepository.GetUserReviewsAsync(
            userId,
            paginationParams.Cursor,
            paginationParams.PageSize,
            cancellationToken
        );

        var dtos = pagedResult.Items.Select(r => r.ToDto()).ToList();

        return Result<PagedResult<ReviewDto>>.Success(new PagedResult<ReviewDto>
        {
            Items = dtos,
            TotalCount = pagedResult.TotalCount,
            NextCursor = pagedResult.NextCursor,
            HasMore = pagedResult.HasMore
        });
    }

    public async Task<Result<RestaurantRatingDto>> GetRestaurantRatingAsync(
        int restaurantId,
        CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant == null)
        {
            return Result<RestaurantRatingDto>.Failure("Restaurant not found", "NOT_FOUND");
        }

        var averageRating = await reviewRepository.GetAverageRatingAsync(restaurantId, cancellationToken);
        var distribution = await reviewRepository.GetRatingDistributionAsync(restaurantId, cancellationToken);
        var totalReviews = distribution.Values.Sum();

        return Result<RestaurantRatingDto>.Success(new RestaurantRatingDto
        {
            RestaurantId = restaurantId,
            AverageRating = Math.Round(averageRating, 2),
            TotalReviews = totalReviews,
            RatingDistribution = distribution
        });
    }

    public async Task<Result<ReviewDto>> CreateAsync(
        string userId,
        CreateReviewDto dto,
        CancellationToken cancellationToken = default)
    {
        // Check if restaurant exists
        var restaurant = await restaurantRepository.GetByIdAsync(dto.RestaurantId, cancellationToken);
        if (restaurant == null)
        {
            return Result<ReviewDto>.Failure("Restaurant not found", "NOT_FOUND");
        }

        // Check if user already reviewed this restaurant
        var hasReviewed = await reviewRepository.HasUserReviewedRestaurantAsync(userId, dto.RestaurantId, cancellationToken);
        if (hasReviewed)
        {
            return Result<ReviewDto>.Failure("You have already reviewed this restaurant", "ALREADY_REVIEWED");
        }

        // Validate order if provided
        if (dto.OrderId.HasValue)
        {
            var order = await orderRepository.GetByIdAsync(dto.OrderId.Value, cancellationToken);
            if (order == null || order.UserId != userId || order.RestaurantId != dto.RestaurantId)
            {
                return Result<ReviewDto>.Failure("Invalid order", "INVALID_ORDER");
            }
        }

        var review = new Review
        {
            UserId = userId,
            RestaurantId = dto.RestaurantId,
            OrderId = dto.OrderId,
            Rating = dto.Rating,
            Title = dto.Title,
            Comment = dto.Comment,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await reviewRepository.AddAsync(review, cancellationToken);

        // Reload with details
        var reviewWithDetails = await reviewRepository.GetWithDetailsAsync(created.Id, cancellationToken);
        return Result<ReviewDto>.Success(reviewWithDetails!.ToDto());
    }

    public async Task<Result<ReviewDto>> UpdateAsync(
        int id,
        string userId,
        UpdateReviewDto dto,
        CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByIdAsync(id, cancellationToken);
        if (review == null)
        {
            return Result<ReviewDto>.Failure("Review not found", "NOT_FOUND");
        }

        if (review.UserId != userId)
        {
            return Result<ReviewDto>.Failure("Access denied", "FORBIDDEN");
        }

        review.Rating = dto.Rating;
        review.Title = dto.Title;
        review.Comment = dto.Comment;
        review.UpdatedAt = DateTime.UtcNow;

        await reviewRepository.UpdateAsync(review, cancellationToken);

        var updated = await reviewRepository.GetWithDetailsAsync(id, cancellationToken);
        return Result<ReviewDto>.Success(updated!.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByIdAsync(id, cancellationToken);
        if (review == null)
        {
            return Result<bool>.Failure("Review not found", "NOT_FOUND");
        }

        if (review.UserId != userId)
        {
            return Result<bool>.Failure("Access denied", "FORBIDDEN");
        }

        await reviewRepository.DeleteAsync(review, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<ReviewDto>> AddResponseAsync(
        int reviewId,
        string responderId,
        CreateReviewResponseDto dto,
        CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetWithDetailsAsync(reviewId, cancellationToken);
        if (review == null)
        {
            return Result<ReviewDto>.Failure("Review not found", "NOT_FOUND");
        }

        var response = new ReviewResponse
        {
            ReviewId = reviewId,
            ResponderId = responderId,
            ResponseText = dto.ResponseText,
            CreatedAt = DateTime.UtcNow
        };

        review.Responses.Add(response);
        await reviewRepository.UpdateAsync(review, cancellationToken);

        var updated = await reviewRepository.GetWithDetailsAsync(reviewId, cancellationToken);
        return Result<ReviewDto>.Success(updated!.ToDto());
    }
}
