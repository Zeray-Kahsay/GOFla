using System;
using FluentValidation;
using GoFla.API.DTOs.Orders;

namespace GoFla.API.Validators;

public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
{
    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(s => new[] { "Pending", "Confirmed", "Preparing", "OutForDelivery", "Delivered", "Cancelled" }
                .Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid order status");
    }
}
