using System;
using GoFla.API.Commons;
using GoFla.API.Data;
using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Repositories;

public class MenuItemRepository(AppDbContext context) : IMenuItemRepository
{
    public async Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.MenuItems
            .Include(m => m.Restaurant)
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<List<MenuItem>> GetByRestaurantAsync(int restaurantId, CancellationToken cancellationToken)
    {
        return await context.MenuItems
            .Include(m => m.Category)
            .Where(m => m.RestaurantId == restaurantId)
            .OrderBy(m => m.Category.SortOrder)
            .ThenBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MenuItem item, CancellationToken cancellationToken)
    {
        context.MenuItems.Add(item);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(MenuItem item, CancellationToken cancellationToken)
    {
        context.MenuItems.Update(item);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MenuItem item, CancellationToken cancellationToken)
    {
       context.MenuItems.Remove(item);
        await context.SaveChangesAsync(cancellationToken);
    }
}
