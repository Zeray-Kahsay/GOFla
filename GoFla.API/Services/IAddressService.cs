using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Address;

namespace GoFla.API.Services;

public interface IAddressService
{
    Task<Result<AddressDto>> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<Result<List<AddressDto>>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<AddressDto>> CreateAsync(string userId, CreateAddressDto dto, CancellationToken cancellationToken = default);
    Task<Result<AddressDto>> UpdateAsync(int id, string userId, UpdateAddressDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> SetDefaultAsync(int id, string userId, CancellationToken cancellationToken = default);
   
}
