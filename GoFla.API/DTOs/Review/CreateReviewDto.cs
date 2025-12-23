using System;

namespace GoFla.API.DTOs.Review;

public record CreateReviewDto
{
    public int RestaurantId { get; init; }
    public int? OrderId { get; init; }
    public int Rating { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
}
