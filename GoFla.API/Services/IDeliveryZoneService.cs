using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Address;

namespace GoFla.API.Services;

public interface IDeliveryZoneService
{
    Task<bool> IsAddressDeliverableAsync(
       double latitude,
       double longitude,
       int restaurantId,
        CancellationToken cancellationToken = default
    );
    Task<Result<DeliveryCheckResultDto>> CheckDeliveryAsync(
        int addressId, 
        string userId, 
        int restaurantId,
        CancellationToken cancellationToken = default);
}
