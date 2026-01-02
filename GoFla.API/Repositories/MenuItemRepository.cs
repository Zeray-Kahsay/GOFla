using System;
using GoFla.API.Data;
using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Repositories;

public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(AppDbContext context) : base(context){}
    
        
    
    public Task<bool> RestaurantExistsAsync(int restaurantId, CancellationToken ct) =>
        _context.Restaurants.AnyAsync(r => r.Id == restaurantId, ct);
    
    
}
