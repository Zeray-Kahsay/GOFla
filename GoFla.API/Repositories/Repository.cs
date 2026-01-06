using System;
using System.Linq.Expressions;
using System.Text;
using GoFla.API.Commons;
using GoFla.API.Data;
using GoFla.API.Domain;
using GoFla.API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<PagedResult<T>> GetPagedAsync<TKey>(
    Expression<Func<T, bool>>? predicate,
    Expression<Func<T, TKey>> orderBy,
    string? cursor,
    int pageSize,
    bool descending = true,
    CancellationToken cancellationToken = default,
    params Expression<Func<T, object>>[] includes)
    where TKey : struct, IComparable<TKey>
    {
        IQueryable<T> query = _dbSet.AsQueryable();

        // Includes
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        // Predicate
        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Parse cursor → TKey?
        TKey? parsedCursor = null;
        if (!string.IsNullOrWhiteSpace(cursor))
        {
            parsedCursor = (TKey)Convert.ChangeType(cursor, typeof(TKey));
        }

        // Apply cursor pagination (ordering + filtering + pageSize + 1)
        var items = await query
            .ApplyCursorPagination(
                orderBy,
                parsedCursor,
                pageSize,
                descending)
            .ToListAsync(cancellationToken);

        // Determine hasMore
        var hasMore = items.Count > pageSize;
        if (hasMore)
        {
            items.RemoveAt(items.Count - 1);
        }

        // Next cursor
        var orderByFunc = orderBy.Compile();
        var nextCursor = hasMore
            ? orderByFunc(items.Last()).ToString()
            : null;

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            HasMore = hasMore,
            NextCursor = nextCursor
        };
    }



    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }


    // Helper methods for cursor encoding/decoding
    private static string EncodeCursor<TKey>(TKey value)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    private static TKey DecodeCursor<TKey>(string cursor)
    {
        var bytes = Convert.FromBase64String(cursor);
        var json = Encoding.UTF8.GetString(bytes);
        return System.Text.Json.JsonSerializer.Deserialize<TKey>(json)!;
    }
}
