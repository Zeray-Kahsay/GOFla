using System;

namespace GoFla.API.DTOs.Orders;

public record UpdateOrderStatusDto
{
    public string Status  { get; set; } = string.Empty;
}
