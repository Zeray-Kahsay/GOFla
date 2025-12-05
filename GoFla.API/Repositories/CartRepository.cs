using System;
using GoFla.API.Data;
using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{

    public CartRepository(AppDbContext context) : base(context) { }
    
    public async Task<Cart?> GetUserCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Items)
                .ThenInclude(ci => ci.MenuItem)
                    .ThenInclude(m => m.Restaurant)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<Cart?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Items)
                .ThenInclude(ci => ci.MenuItem)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
