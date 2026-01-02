using System;
using FluentValidation;
using GoFla.API.DTOs.Address;

namespace GoFla.API.Validators;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(200).WithMessage("Street must not exceed 200 characters");
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");
        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100).WithMessage("State must not exceed 100 characters");
        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required")
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters");
        RuleFor(x => x.CountryCode)
            .NotEmpty().WithMessage("Country code is required")
            .Length(2).WithMessage("Country code must be 2 letters");
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
    }
}
