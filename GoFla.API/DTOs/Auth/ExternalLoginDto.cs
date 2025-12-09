namespace GoFla.API.DTOs.Auth;

public record ExternalLoginDto
{
    public string  Provider  { get; set; } = string.Empty;
    public string  AccessToken  { get; set; } = string.Empty;
}
