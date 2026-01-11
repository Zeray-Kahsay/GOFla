using System;
using GoFla.API.Data;
using GoFla.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }


    public async Task<List<Category>> GetByRestaurantIdAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .Where(c => c.RestaurantId == restaurantId)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }


    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await context.Categories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }



    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        context.Categories.Update(category);
        await SaveAllAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        context.Categories.Remove(category);
        return await SaveAllAsync(cancellationToken);
    }

    public async Task<bool> SaveAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
