using System;

namespace GoFla.API.DTOs.Address;

public record DeliveryCheckResultDto
{
    public bool  IsDeliverable  { get; set; }
    public string? Reason { get; set; }
}
