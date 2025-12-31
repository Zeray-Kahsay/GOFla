using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Address;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class DeliveryZoneService( 
    IRestaurantRepository restaurantRepository,
    IRepository<Address> repository
    ) : IDeliveryZoneService
{
    public async Task<bool> IsAddressDeliverableAsync(double latitude, double longitude, int restaurantId, CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant == null) return false;

        var distanceKm = GeoUtils.CalculateDistanceKm(
            latitude, longitude,
            restaurant.Address.Latitude, restaurant.Address.Longitude  // TODO: inlude Address
        );

        return distanceKm <= restaurant.DeliveryRadiusKm;
    }


    public async Task<Result<DeliveryCheckResultDto>> CheckDeliveryAsync(int addressId, string userId, int restaurantId, CancellationToken cancellationToken = default)
    {
        var address = await repository.GetByIdAsync(addressId, cancellationToken);

        if (address is null)
        {
            return Result<DeliveryCheckResultDto>.Failure("Address not found", "NOT_FOUND");
        }

        if (address.UserId != userId)
        {
            return Result<DeliveryCheckResultDto>.Failure("Access denied", "FORBIDDEN");
        }

        var isDeliverable =  await IsAddressDeliverableAsync(
            address.Latitude,
            address.Longitude,
            restaurantId,
            cancellationToken
        );

        return Result<DeliveryCheckResultDto>.Success(new DeliveryCheckResultDto
        {
            IsDeliverable = isDeliverable,
            Reason = isDeliverable ? null : "OUT_OF_DELIVERY_ZONE"
        });
    }

    private static class GeoUtils
    {
        private const double EarthRadiusKm = 6371;

        public static double CalculateDistanceKm(
            double lat1, double lon1,
            double lat2, double lon2)
        {
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) *
                Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }


    // static string Normalize(string value) => value.Replace(" ", "").ToUpperInvariant();
    // static bool Matches(string pattern, string postal)
    // {
    //     if (pattern.Contains('-'))
    //     {
    //         var parts = pattern.Split('-');
    //         return string.Compare(postal, parts[0]) >= 0 &&
    //                string.Compare(postal, parts[1]) <= 0;
    //     }

    //     return postal.StartsWith(pattern);
    // }
}
