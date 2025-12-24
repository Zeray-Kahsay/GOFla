using System;
using System.Text.RegularExpressions;
using FluentValidation;
using GoFla.API.DTOs.Address;

namespace GoFla.API.Validators;

public class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
{
    public CreateAddressDtoValidator()
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

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .Length(2);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x)
            .Must(dto => IsValidPostalCode(dto.CountryCode, dto.PostalCode))
            .WithMessage("Invalid postal code for selected country");



    }

    private static bool IsValidPostalCode(string countryCode, string postalCode)
    {
        countryCode = countryCode.ToUpperInvariant();

        return countryCode switch
        {
            "US" => Regex.IsMatch(postalCode, @"^\d{5}(-\d{4})?$"),
            "NO" => Regex.IsMatch(postalCode, @"^\d{4}$"),
            "DE" => Regex.IsMatch(postalCode, @"^\d{5}$"),
            "GB" => Regex.IsMatch(postalCode,
                     @"^[A-Z]{1,2}\d[A-Z\d]?\s?\d[A-Z]{2}$",
                     RegexOptions.IgnoreCase),

            _ => true // allow unknown countries
        };
    }

}
