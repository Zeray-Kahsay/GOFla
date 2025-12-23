using System;

namespace GoFla.API.DTOs.Review;

public record ReviewDto
{
    public int Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string? UserProfileImage { get; init; }
    public int RestaurantId { get; init; }
    public string RestaurantName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public List<ReviewResponseDto> Responses { get; init; } = new();
}
