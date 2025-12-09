namespace GoFla.API.DTOs.Auth;

public record RefreshTokenDto
{
    public string  RefreshToken  { get; set; } = string.Empty;
}
