using System;
using FluentValidation;
using GoFla.API.DTOs.Address;

namespace GoFla.API.Validators;

public class UpdateAddressDtoValidator : AbstractValidator<UpdateAddressDto>
{
    public UpdateAddressDtoValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("Address label is required")
            .MaximumLength(50).WithMessage("Label must not exceed 50 characters");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(200).WithMessage("Street must not exceed 200 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(50).WithMessage("State must not exceed 50 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code is required")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Invalid zip code format (e.g., 12345 or 12345-6789)");
    }
}
