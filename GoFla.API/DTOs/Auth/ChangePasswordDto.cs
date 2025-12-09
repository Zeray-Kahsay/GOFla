namespace GoFla.API.DTOs.Auth;

public record ChangePasswordDto
{
    public string  CurrentPassword  { get; set; } = string.Empty;
    public string  NewPassword  { get; set; } = string.Empty;
}
