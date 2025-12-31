using System;
using FluentValidation;
using GoFla.API.DTOs.Restaurants;

namespace GoFla.API.Validators;

public class UpdateRestaurantDtoValidator : AbstractValidator<UpdateRestaurantDto>
{
    public UpdateRestaurantDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Restaurant name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");

        RuleFor(x => x.DeliveryFee)
            .GreaterThanOrEqualTo(0).WithMessage("Delivery fee must be zero or positive");

        RuleFor(x => x.EstimatedDeliveryTime)
            .GreaterThan(0).WithMessage("Estimated delivery time must be greater than 0")
            .LessThanOrEqualTo(300).WithMessage("Estimated delivery time must not exceed 300 minutes");

        // Nested Address validation
        RuleFor(x => x.AddressDto).NotNull().WithMessage("Address is required");
        RuleFor(x => x.AddressDto.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(200).WithMessage("Street must not exceed 200 characters");
        RuleFor(x => x.AddressDto.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");
        RuleFor(x => x.AddressDto.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100).WithMessage("State must not exceed 100 characters");
        RuleFor(x => x.AddressDto.PostalCode)
            .NotEmpty().WithMessage("Postal code is required")
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters");
        RuleFor(x => x.AddressDto.CountryCode)
            .NotEmpty().WithMessage("Country code is required")
            .Length(2).WithMessage("Country code must be 2 letters");
        RuleFor(x => x.AddressDto.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");
        RuleFor(x => x.AddressDto.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
    }
}


// using System;
// using FluentValidation;
// using GoFla.API.DTOs.Restaurants;

// namespace GoFla.API.Validators;

// public class UpdateRestaurantDtoValidator : AbstractValidator<UpdateRestaurantDto>
// {
//     public UpdateRestaurantDtoValidator()
//     {
//         RuleFor(x => x.Name)
//             .NotEmpty().WithMessage("Restaurant name is required")
//             .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

//         RuleFor(x => x.Description)
//             .NotEmpty().WithMessage("Description is required")
//             .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

//         RuleFor(x => x.Address)
//             .NotEmpty().WithMessage("Address is required")
//             .MaximumLength(200).WithMessage("Address must not exceed 200 characters");

//         RuleFor(x => x.Phone)
//             .NotEmpty().WithMessage("Phone is required")
//             .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format");

//         RuleFor(x => x.DeliveryFee)
//             .GreaterThanOrEqualTo(0).WithMessage("Delivery fee must be zero or positive");

//         RuleFor(x => x.EstimatedDeliveryTime)
//             .GreaterThan(0).WithMessage("Estimated delivery time must be greater than 0")
//             .LessThanOrEqualTo(300).WithMessage("Estimated delivery time must not exceed 300 minutes");
//     }
// }
