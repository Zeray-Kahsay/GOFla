using System;

namespace GoFla.API.Services;

public interface IDeliveryZoneService
{
    Task<bool> IsAddressDeliverableAsync(
        string CountryCode,
        string PostalCode,
        CancellationToken cancellationToken = default
    );
}
