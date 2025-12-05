using System;
using GoFla.API.Commons;
using GoFla.API.Domain;

namespace GoFla.API.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<Order>> GetUserOrdersAsync(string userId, string? cursor, int pageSize, CancellationToken cancellationToken = default);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
}
