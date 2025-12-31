using System;
using System.Linq.Expressions;
using GoFla.API.Commons;
using GoFla.API.Data;
using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Repositories;

public class RestaurantRepository : Repository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(AppDbContext context) : base(context){}

    public async Task<PagedResult<Restaurant>> GetPagedWithDetailsAsync(
        Expression<Func<Restaurant,
         bool>> predicate, 
         Expression<Func<Restaurant, 
         int>> orderBy, 
         string? cursor, 
         int pageSize, 
         CancellationToken cancellationToke = default)
    {
         IQueryable<Restaurant> query = _dbSet
            .Include(r => r.Address)
            .Where(predicate)
            .OrderBy(orderBy);

        if (!string.IsNullOrEmpty(cursor))
        {
            var lastId = int.Parse(cursor);
            query = query.Where(r => r.Id > lastId);
        }

        var items = await query
                .Take(pageSize + 1)
                .ToListAsync(cancellationToke);
        
        var hasMore = items.Count > pageSize;

        if (hasMore)
            items.RemoveAt(items.Count - 1);
        
        return new PagedResult<Restaurant>
        {
            Items = items,
            HasMore = hasMore,
            NextCursor = hasMore ? items.Last().Id.ToString() : null,
            TotalCount = await _dbSet.CountAsync(predicate, cancellationToke)
        };
    }


    public async Task<Restaurant?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
       return await _dbSet
            .Include(r => r.MenuItems)
            .Include(r => r.Address)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
}
