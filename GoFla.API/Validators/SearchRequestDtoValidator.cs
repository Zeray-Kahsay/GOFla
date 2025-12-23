using System;
using FluentValidation;
using GoFla.API.DTOs.Search;

namespace GoFla.API.Validators;

public class SearchRequestDtoValidator : AbstractValidator<SearchRequestDto>
{
    public SearchRequestDtoValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Search query is required")
            .MinimumLength(2).WithMessage("Query must be at least 2 characters")
            .MaximumLength(100).WithMessage("Query must not exceed 100 characters");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue)
            .WithMessage("Minimum price must be zero or positive");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue)
            .WithMessage("Maximum price must be zero or positive")
            .GreaterThan(x => x.MinPrice).When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
            .WithMessage("Maximum price must be greater than minimum price");

        RuleFor(x => x.MinRating)
            .InclusiveBetween(1, 5).When(x => x.MinRating.HasValue)
            .WithMessage("Rating must be between 1 and 5");
    }
}
