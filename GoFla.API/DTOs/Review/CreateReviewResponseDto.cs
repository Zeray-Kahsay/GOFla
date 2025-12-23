using System;

namespace GoFla.API.DTOs.Review;

public record CreateReviewResponseDto
{
    public string ResponseText { get; init; } = string.Empty;
}
