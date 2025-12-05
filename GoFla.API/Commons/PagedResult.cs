namespace GoFla.API.Commons;

public class PagedResult<T>
{
    public List<T> Items  { get; set; } = new();
    public int  TotalCount { get; set; }
    public string? NextCursor { get; set; }
    public bool HasMore { get; set; }
}
