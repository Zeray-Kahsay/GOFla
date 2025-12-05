namespace GoFla.API.Commons;

public record PaginationParams
{
    public string? Cursor { get; init; }
    public int PageSize { get; init; } = 20; 
}
