using System;

namespace GoFla.API.DTOs.Search;

public record SearchRequestDto
{
    public string Query { get; init; } = string.Empty;
    public string? Category { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public int? MinRating { get; init; }
    public bool? IsAvailable { get; init; }
    public string? SortBy { get; init; } // "relevance", "rating", "price", "distance"
    public int PageSize { get; init; } = 20;
    public string? Cursor { get; init; }
}
