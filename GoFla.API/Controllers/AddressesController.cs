using GoFla.API.DTOs.Address;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class AddressesController(IAddressService addressService, IDeliveryZoneService deliveryZoneService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetUserAddresses(CancellationToken cancellationToken)
    {
        string userId = GetUserId();

        return HandleResult(await addressService.GetUserAddressesAsync(userId, cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAddressById(int id, CancellationToken cancellationToken)
    {
        string userId = GetUserId();

        return HandleResult(await addressService.GetByIdAsync(id, userId, cancellationToken));

    }

    [HttpPost]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto dto, CancellationToken cancellationToken)
    {
        string userId = GetUserId();

        return HandleResult(await addressService.CreateAsync(userId, dto, cancellationToken));

        
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAddressDto dto, CancellationToken cancellationToken)
    {
        string userId = GetUserId();

        return HandleResult(await addressService.UpdateAsync(id, userId, dto, cancellationToken));

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(int id, CancellationToken cancellationToken)
    {
        string userId = GetUserId();

        return HandleResult(await addressService.DeleteAsync(id, userId, cancellationToken));

    
    }

    [HttpPost("{id}/setDefault-address")]
    public async Task<IActionResult> SetDefaultAddress(int id, CancellationToken cancellationToken)
    {
        string userId = GetUserId();

        return HandleResult(await addressService.SetDefaultAsync(id, userId, cancellationToken));
        
    }


    [HttpGet("{addressId}/check-delivery-address")]
    public async Task<IActionResult> CheckDelivery(
    int addressId,
    [FromQuery] int restaurantId,
    CancellationToken cancellationToken)
    {
        string userId = GetUserId();

        return HandleResult(await deliveryZoneService.CheckDeliveryAsync(addressId, userId, restaurantId, cancellationToken));


        // if (!result.IsSuccess)
        //     return BadRequest(result);

        // return Ok(new
        // {
        //     isDeliverable = result.Data!.IsDeliverable,
        //     reason = result.Data.Reason
        // });
    }


    private static double GeoDistanceKm(
    double lat1, double lon1,
    double lat2, double lon2)
    {
        const double R = 6371;
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(DegreesToRadians(lat1)) *
            Math.Cos(DegreesToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }






}
