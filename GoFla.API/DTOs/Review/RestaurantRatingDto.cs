using System;

namespace GoFla.API.DTOs.Review;

public record RestaurantRatingDto
{
    public int RestaurantId { get; init; }
    public double AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public Dictionary<int, int> RatingDistribution { get; init; } = [];
}
