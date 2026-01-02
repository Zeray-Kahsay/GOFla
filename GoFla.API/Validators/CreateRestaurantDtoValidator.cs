using System;
using FluentValidation;
using GoFla.API.DTOs.Restaurants;

namespace GoFla.API.Validators;

public class CreateRestaurantDtoValidator : AbstractValidator<CreateRestaurantDto>
{
    public CreateRestaurantDtoValidator()
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
        RuleFor(x => x.AddressDto).NotNull().SetValidator(new AddressDtoValidator());

    }
}
