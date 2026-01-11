using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Category;

namespace GoFla.API.Repositories;


public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<MenuItem>> GetByRestaurantAsync(int restaurantId, CancellationToken cancellationToken);
    Task AddAsync(MenuItem item, CancellationToken cancellationToken);
    Task UpdateAsync(MenuItem item, CancellationToken cancellationToken);
    Task DeleteAsync(MenuItem item, CancellationToken cancellationToken);
}

