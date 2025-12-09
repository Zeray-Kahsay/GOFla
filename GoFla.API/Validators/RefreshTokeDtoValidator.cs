using FluentValidation;
using GoFla.API.DTOs.Auth;

namespace GoFla.API.Validators;

public class RefreshTokeDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokeDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");  
    }
}
