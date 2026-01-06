using System;
using GoFla.API.Commons;
using GoFla.API.Data;
using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
           .Include(o => o.Items)
               .ThenInclude(oi => oi.MenuItem)
           .Include(o => o.Restaurant)
           .Include(o => o.DeliveryAddress)
           .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<PagedResult<Order>> GetUserOrdersAsync(string userId, string? cursor, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(
        predicate: o => o.UserId == userId,
        orderBy: o => o.CreatedAt,
        descending: true,
        cursor: cursor,
        pageSize: pageSize,
        cancellationToken: cancellationToken
     );
    }

    public async Task<Order?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
           .Include(o => o.Items)
               .ThenInclude(oi => oi.MenuItem)
           .Include(o => o.Restaurant)
           .Include(o => o.DeliveryAddress)
           .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
