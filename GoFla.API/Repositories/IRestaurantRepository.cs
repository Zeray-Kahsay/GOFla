using System;
using GoFla.API.Domain;

namespace GoFla.API.Repositories;

public interface IRestaurantRepository : IRepository<Restaurant>
{
    Task<Restaurant?> GetWithMenuItemsAsync(int id, CancellationToken cancellationToken = default);
}
