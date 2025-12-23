using System;
using FluentValidation;
using GoFla.API.DTOs.Review;

namespace GoFla.API.Validators;

public class CreateReviewResponseDtoValidator : AbstractValidator<CreateReviewResponseDto>
{
    public CreateReviewResponseDtoValidator()
    {
        RuleFor(x => x.ResponseText)
            .NotEmpty().WithMessage("Response text is required")
            .MinimumLength(10).WithMessage("Response must be at least 10 characters")
            .MaximumLength(500).WithMessage("Response must not exceed 500 characters");
    }
}
