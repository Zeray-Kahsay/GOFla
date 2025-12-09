using FluentValidation;
using GoFla.API.DTOs.Auth;

namespace GoFla.API.Validators;

public class ExternalLoginDtoValidator : AbstractValidator<ExternalLoginDto>    
{
    public ExternalLoginDtoValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Provider is required.")
            .Must(p => new[]{"google", "facebook", "github", "linkedin"}.Contains(p.ToLower()))
            .WithMessage("Provider must be one of: google, facebook, github, linkedin");
        
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is requird");
    }
}
