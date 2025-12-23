using System;
using FluentValidation;
using GoFla.API.DTOs.Review;

namespace GoFla.API.Validators;

public class UpdateReviewDtoValidator : AbstractValidator<UpdateReviewDto>
{
    public UpdateReviewDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required")
            .MinimumLength(10).WithMessage("Comment must be at least 10 characters")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters");
    }
}
