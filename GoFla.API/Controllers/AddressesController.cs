using System;
using GoFla.API.Data;
using GoFla.API.DTOs.Address;
using GoFla.API.Extensions;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Controllers;

public class AddressesController(IAddressService addressService, AppDbContext context) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetUserAddresses(CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await addressService.GetUserAddressesAsync(userId, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAddressById(int id, CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await addressService.GetByIdAsync(id, userId, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDto dto, CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await addressService.CreateAsync(userId, dto, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAddressDto dto, CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await addressService.UpdateAsync(id, userId, dto, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(int id, CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await addressService.DeleteAsync(id, userId, cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id}/setDefault-address")]
    public async Task<IActionResult> SetDefaultAddress(int id, CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await addressService.SetDefaultAsync(id, userId, cancellationToken);
        return Ok(result);
    }


    [HttpGet("{addressId}/check-delivery-address")]
    public async Task<IActionResult> CheckDelivery(
    int addressId,
    CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var address = await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address is null || address.Latitude == null || address.Longitude == null)
            return Ok(new { isDeliverable = false });

        const double restaurantLat = 59.9139;
        const double restaurantLng = 10.7522;
        const double maxKm = 10;

        var distance = GeoDistanceKm(
            address.Latitude.Value,
            address.Longitude.Value,
            restaurantLat,
            restaurantLng
        );

        return Ok(new { isDeliverable = distance <= maxKm });
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
