using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using Stripe;

namespace GoFla.API.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task CreateOrderAsync(Order order, CancellationToken cancellationToken = default); // TODO: the parameters
     Task<bool> SaveChangesAsync(CancellationToken ct);
     Task<Order?> GetByPaymentIntentIdAsync(string intentId, CancellationToken ct);
     Task<List<Order>> GetExpiredPendingPaymentOrderAsync();
    // Task<Order?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    // Task<PagedResult<Order>> GetUserOrdersAsync(string userId, string? cursor, int pageSize, CancellationToken cancellationToken = default);
     Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
}
