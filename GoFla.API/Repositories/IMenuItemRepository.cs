using System;
using GoFla.API.Domain;

namespace GoFla.API.Repositories;


// Not used repo --- will be deleted 
public interface IMenuItemRepository : IRepository<MenuItem>
{
    Task<bool> RestaurantExistsAsync(int restaurantId, CancellationToken ct);
}
