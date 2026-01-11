using System;
using GoFla.API.Domain;

namespace GoFla.API.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Category>> GetByRestaurantIdAsync(int restaurantId, CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> SaveAllAsync(CancellationToken cancellationToken = default);
}
