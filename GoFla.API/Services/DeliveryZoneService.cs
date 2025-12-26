using System;
using GoFla.API.Data;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Services;

public class DeliveryZoneService(AppDbContext context) : IDeliveryZoneService
{
    public async Task<bool> IsAddressDeliverableAsync(string CountryCode, string PostalCode, CancellationToken cancellationToken = default)
    {
        var normalizedPostal = Normalize(PostalCode);

        var zones = await context.DeliveryZones
            .Where(z => z.CountryCode == CountryCode && z.IsActive)
            .ToListAsync(cancellationToken);
        
        foreach(var zone in zones)
        {
            if (Matches(zone.PostalCodePattern, normalizedPostal))
                return true;
        }

        return false;
    }

    static string Normalize(string value) => value.Replace(" ", "").ToUpperInvariant();
    static bool Matches (string pattern, string postal)
    {
        if (pattern.Contains('-'))
        {
            var parts = pattern.Split('-');
            return string.Compare(postal, parts[0]) >= 0 &&
                   string.Compare(postal, parts[1]) <= 0;
        }

        return postal.StartsWith(pattern);
    }
}
