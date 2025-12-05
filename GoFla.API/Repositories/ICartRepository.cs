using System;
using GoFla.API.Domain;

namespace GoFla.API.Repositories;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetUserCartAsync(string userId, CancellationToken cancellationToken = default);
    Task<Cart?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default);
}
