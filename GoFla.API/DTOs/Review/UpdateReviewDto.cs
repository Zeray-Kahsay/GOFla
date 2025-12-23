using System;

namespace GoFla.API.DTOs.Review;

public record UpdateReviewDto
{
    public int Rating { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
}
