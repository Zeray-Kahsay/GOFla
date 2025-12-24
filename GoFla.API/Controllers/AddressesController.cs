using System;
using GoFla.API.DTOs.Address;
using GoFla.API.Extensions;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class AddressesController (IAddressService addressService) : BaseController
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

    [HttpPost("{id}/set-default")]
    public async Task<IActionResult> SetDefaultAddress(int id, CancellationToken cancellationToken)
    {
        string userId = User.GetUserId() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await addressService.SetDefaultAsync(id, userId, cancellationToken);
        return Ok(result);
    }



}
