using System;
using FluentValidation;
using GoFla.API.DTOs.Orders;

namespace GoFla.API.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.DeliveryAddressId)
            .GreaterThan(0).WithMessage("Valid delivery address is required");

        RuleFor(x => x.PaymentMethodId)
            .MaximumLength(100).WithMessage("Payment method ID is too long");
    }
}
