using System;
using FluentValidation;
using GoFla.API.DTOs.Cart;

namespace GoFla.API.Validators;

public class UpdateCartItemDtoValidator : AbstractValidator<UpdateCartItemDto>
{
    public UpdateCartItemDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(50).WithMessage("Quantity cannot exceed 50");
    }
}
