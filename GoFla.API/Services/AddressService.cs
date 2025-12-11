using System;
using GoFla.API.Commons;
using GoFla.API.Data;
using GoFla.API.Domain;
using GoFla.API.DTOs.Address;
using GoFla.API.Extensions;
using GoFla.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Services;

public class AddressService(IRepository<Address> addressRepository, AppDbContext context) : IAddressService
{
    public async Task<Result<AddressDto>> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var address = await addressRepository.GetByIdAsync(id, cancellationToken);
        if (address is null)
        {
            return Result<AddressDto>.Failure("Address not found", "NOT_FOUND");
        }

        if (address.UserId != userId)
        {
            return Result<AddressDto>.Failure("Access denied", "FORBIDDEN");
        }

        return Result<AddressDto>.Success(address.ToAddressDto());
    }


    public async Task<Result<List<AddressDto>>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var addresses = await context.Addresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        var addressDtos = addresses.Select(a => a.ToAddressDto()).ToList();

        return Result<List<AddressDto>>.Success(addressDtos);
    }


    public async Task<Result<AddressDto>> CreateAsync(string userId, CreateAddressDto dto, CancellationToken cancellationToken = default)
    {
        // If this is set as default, unset all other default addresses
        if (dto.IsDefault)
        {
            await UnsetAllDefaultAddressesAsync(userId, cancellationToken);
        }

        var address = new Address
        {
            UserId = userId,
            Label = dto.Label,
            Street = dto.Street,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            IsDefault = dto.IsDefault,
            CreatedAt = DateTime.UtcNow
        };

        var createdAddress = await addressRepository.AddAsync(address, cancellationToken);

        return Result<AddressDto>.Success(createdAddress.ToAddressDto());
    }


    public async Task<Result<AddressDto>> UpdateAsync(int id, string userId, UpdateAddressDto dto, CancellationToken cancellationToken = default)
    {
        var address = await addressRepository.GetByIdAsync(id, cancellationToken);
        if (address is null)
        {
            return Result<AddressDto>.Failure("Address not found.", "NOT_FOUND");
        }

        if (address.UserId != userId)
        {
            return Result<AddressDto>.Failure("Access denied", "FORBIDDEN");
        }

        // If this is set as default, unset all other default addresses
        if (dto.IsDefault && !address.IsDefault)
        {
            await UnsetAllDefaultAddressesAsync(userId, cancellationToken);
        }

        address.Label = dto.Label;
        address.Street = dto.Street;
        address.City = dto.City;
        address.State = dto.State;
        address.ZipCode = dto.ZipCode;
        address.IsDefault = dto.IsDefault;

        await addressRepository.UpdateAsync(address, cancellationToken);

        return Result<AddressDto>.Success(address.ToAddressDto());


    }


    public async Task<Result<bool>> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var address = await addressRepository.GetByIdAsync(id, cancellationToken);
        if (address is null)
        {
            return Result<bool>.Failure("Address not found", "NOT_FOUND");
        }

        if (address.UserId != userId)
        {
            return Result<bool>.Failure("Access denied", "FORBIDDEN");
        }

        await addressRepository.DeleteAsync(address, cancellationToken);

        return Result<bool>.Success(true);
    }



    public async Task<Result<bool>> SetDefaultAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var address = await addressRepository.GetByIdAsync(id, cancellationToken);
        if (address is null)
        {
            return Result<bool>.Failure("Address not found", "NOT_FOUND");
        }

        if (address.UserId != userId)
        {
            return Result<bool>.Failure("Access denied", "FORBIDDEN");
        }

        await UnsetAllDefaultAddressesAsync(userId, cancellationToken);

        address.IsDefault = true;
        await addressRepository.UpdateAsync(address, cancellationToken);

        return Result<bool>.Success(true);
    }


    // Helper method to unset all default addresses for a user
    private async Task UnsetAllDefaultAddressesAsync(string userId, CancellationToken cancellationToken)
    {
        var defaultAddresses = await context.Addresses
            .Where(a => a.UserId == userId && a.IsDefault)
            .ToListAsync(cancellationToken);

        foreach (var addr in defaultAddresses)
        {
            addr.IsDefault = false;
        }
        await context.SaveChangesAsync(cancellationToken);
    }

}
