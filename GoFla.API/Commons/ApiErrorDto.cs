namespace GoFla.API.Commons;

public class ApiErrorDto
{
    public string  Message  { get; init; } = string.Empty;
    public string ErrorCode  { get; init; } = string.Empty;
    public Dictionary<string,string[]>? ValidationErrors  { get; init; }
    public string? StackTrace  { get; init; }
    public DateTime Timestamp  { get; init; } = DateTime.UtcNow;
}
