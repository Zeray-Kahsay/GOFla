using System;
using GoFla.API.Commons;
using GoFla.API.Data;
using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace GoFla.API.Repositories;

public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository
{
    public async Task CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
            await _dbSet.AddAsync(order, cancellationToken);
    }

    public async Task<Order?> GetByPaymentIntentIdAsync(string intentId, CancellationToken ct)
    {
        return await _dbSet.FirstOrDefaultAsync(o => o.PaymentIntentId == intentId, ct);
    }

    public async Task<List<Order>> GetExpiredPendingPaymentOrderAsync()
    {
        return await _dbSet
            .Where(o => o.Status == OrderStatus.PendingPayment && o.PaymentExpiresAt < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<bool> SaveChangesAsync(CancellationToken ct)
    {
        return await context.SaveChangesAsync(ct) > 0;
    }







    // public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    // {
    //     return await _dbSet
    //        .Include(o => o.Items)
    //        //.ThenInclude(oi => oi.MenuItem)
    //        .Include(o => o.Restaurant)
    //        //.Include(o => o.DeliveryAddress)
    //        .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    // }

    // public async Task<PagedResult<Order>> GetUserOrdersAsync(string userId, string? cursor, int pageSize, CancellationToken cancellationToken = default)
    // {
    //     return await GetPagedAsync(
    //     predicate: o => o.CustomerId == userId,
    //     orderBy: o => o.CreatedAt,
    //     descending: true,
    //     cursor: cursor,
    //     pageSize: pageSize,
    //     cancellationToken: cancellationToken
    //  );
    // }

    // public async Task<Order?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    // {
    //     return await _dbSet
    //        .Include(o => o.Items)
    //        //.ThenInclude(oi => oi.MenuItem)
    //        .Include(o => o.Restaurant)
    //        //.Include(o => o.DeliveryAddress)
    //        .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    // }


}
