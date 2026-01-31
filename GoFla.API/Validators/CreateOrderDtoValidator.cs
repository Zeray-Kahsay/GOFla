using System;
using FluentValidation;
using GoFla.API.DTOs.Orders;

namespace GoFla.API.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderDtoValidator()
    {

    }
}
