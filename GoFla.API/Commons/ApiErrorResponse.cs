namespace GoFla.API.Commons;

public record ApiErrorResponse
{
    public ApiErrorResponse(string errorCode, string message, Dictionary<string,string[]>? validationErrors = null)
    {
        Message = message;
        ErrorCode = errorCode;
        ValidationErrors = validationErrors;
    }
    
    public string  Message  { get; init; } = string.Empty;
    public string ErrorCode  { get; init; } = string.Empty;
    public Dictionary<string,string[]>? ValidationErrors  { get; init; }
    public string? StackTrace  { get; init; }
    public DateTime Timestamp  { get; init; } = DateTime.UtcNow;
}
