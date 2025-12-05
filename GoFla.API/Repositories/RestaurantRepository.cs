using System;
using GoFla.API.Data;
using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Repositories;

public class RestaurantRepository : Repository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(AppDbContext context) : base(context){}

    public async Task<Restaurant?> GetWithMenuItemsAsync(int id, CancellationToken cancellationToken = default)
    {
       return await _dbSet
            .Include(r => r.MenuItems)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
}
