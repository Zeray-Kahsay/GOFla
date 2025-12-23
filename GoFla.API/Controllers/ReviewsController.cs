using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Review;
using GoFla.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class ReviewsController (IReviewService reviewService) : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetByIdAsync(id, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("restaurant/{restaurantId}")]
    public async Task<IActionResult> GetRestaurantReviews(
        int restaurantId,
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await reviewService.GetRestaurantReviewsAsync(restaurantId, paginationParams, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("restaurant/{restaurantId}/rating")]
    public async Task<IActionResult> GetRestaurantRating(int restaurantId, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetRestaurantRatingAsync(restaurantId, cancellationToken);
        return HandleResult(result);
    }

    [Authorize]
    [HttpGet("my-reviews")]
    public async Task<IActionResult> GetMyReviews(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var result = await reviewService.GetUserReviewsAsync(GetUserId(), paginationParams, cancellationToken);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto, CancellationToken cancellationToken)
    {
        var result = await reviewService.CreateAsync(GetUserId(), dto, cancellationToken);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto, CancellationToken cancellationToken)
    {
        var result = await reviewService.UpdateAsync(id, GetUserId(), dto, cancellationToken);
        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await reviewService.DeleteAsync(id, GetUserId(), cancellationToken);
        return HandleResult(result);
    }

    [Authorize]
    [HttpPost("{id}/responses")]
    public async Task<IActionResult> AddResponse(
        int id,
        [FromBody] CreateReviewResponseDto dto,
        CancellationToken cancellationToken)
    {
        var result = await reviewService.AddResponseAsync(id, GetUserId(), dto, cancellationToken);
        return HandleResult(result);
    }
}
