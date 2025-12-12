using System;
using FluentValidation;
using GoFla.API.DTOs.Cart;

namespace GoFla.API.Validators;

public class AddToCartDtoValidator : AbstractValidator<AddToCartDto>
{
    public AddToCartDtoValidator()
    {
        RuleFor(x => x.MenuItemId)
            .GreaterThan(0).WithMessage("Valid menu item ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(50).WithMessage("Cannot add more than 50 items at once");

        RuleFor(x => x.SpecialInstructions)
            .MaximumLength(500).WithMessage("Special instructions must not exceed 500 characters");
    }
}
