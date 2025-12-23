using System;

namespace GoFla.API.DTOs.Review;

public record ReviewResponseDto
{
    public int Id { get; init; }
    public string ResponderName { get; init; } = string.Empty;
    public string ResponseText { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
