namespace GoFla.API.Exceptions;

public class AppException : Exception
{
    public string ErrorCode { get;}

    public AppException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }


}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, "NOT_FOUND"){}
    
}

public class ValidationException : AppException
{
    public Dictionary<string, string[]> ValidationErrors { get; }
    public ValidationException(Dictionary<string, string[]> errors) : base("Validation failed", "VALIDATION_ERROR")
    {
        ValidationErrors = errors;
    }
    
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message, "UNAUTHORIZED"){}
    
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message, "FORBIDDEN"){}
    
}
