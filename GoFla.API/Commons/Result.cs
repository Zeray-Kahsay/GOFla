namespace GoFla.API.Commons;

public class Result<T>
{
    public bool IsSuccess  { get; }
    public T? Data  { get; }
    public string?  ErrorMessage { get; }
    public string?  ErrorCode  { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; }

    private Result(bool isSuccess, T? data, string? errorMessage,string? errorCode, Dictionary<string, string[]>? validationErrors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        ValidationErrors = validationErrors;
    }

    public static Result<T> Success(T data) => new (true, data, null, null);
    public static Result<T> Failure(string errorMessage, string errorCode) => 
        new(false, default, errorMessage, errorCode);
    
    public static Result<T> ValidationFailure(Dictionary<string,string[]> validationErrors) => 
        new(false, default, "Validation failed", "VALIDATION_ERROR", validationErrors);
    
}
