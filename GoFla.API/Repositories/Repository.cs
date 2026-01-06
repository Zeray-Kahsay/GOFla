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

        // Parse cursor → TKey?
        TKey? parsedCursor = null;
        if (!string.IsNullOrWhiteSpace(cursor))
        {
            parsedCursor = (TKey)Convert.ChangeType(cursor, typeof(TKey));
        }

        // Apply cursor pagination (ordering + filtering + pageSize + 1)
        var items = await query
            .AppyCursorPagination(
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
            TotalCount = await query.CountAsync(cancellationToken),
            HasMore = hasMore,
            NextCursor = nextCursor
        };
    }


    // public async Task<PagedResult<T>> GetPagedAsync<TKey>(
    // Expression<Func<T, bool>>? predicate,
    // Expression<Func<T, TKey>> orderBy,
    // string? cursor,
    // int pageSize,
    // bool descending = true,
    // CancellationToken cancellationToken = default,
    // params Expression<Func<T, object>>[] includes)
    // where TKey : IComparable<TKey>
    // {
    //     IQueryable<T> query = _dbSet.AsQueryable();


    //     // Apply includes
    //     foreach (var include in includes)
    //     {
    //         query = query.Include(include);
    //     }

    //     if (predicate is not null)
    //         query = query.Where(predicate);

    //     query = descending
    //         ? query.OrderByDescending(orderBy)
    //         : query.OrderBy(orderBy);

    //     DateTime parsedCursor;



    //     var parsedCursor = cursor is null
    //         ? default
    //         : (TKey)Convert.ChangeType(cursor, typeof(TKey));

    //     var orderByFunc = orderBy.Compile();

    //     if (parsedCursor is not null)
    //     {
    //         query = descending
    //           ? query.Where(e => orderByFunc(e).CompareTo(parsedCursor) < 0)
    //           : query.Where(e => orderByFunc(e).CompareTo(parsedCursor) > 0);
    //     }

    //     DateTime
    //     var items = await query
    //         .AppyCursorPagination(orderBy, parsedCursor, pageSize, descending)
    //         .ToListAsync(cancellationToken);

    //     var hasMore = items.Count > pageSize;
    //     if (hasMore)
    //         items.RemoveAt(items.Count - 1);


    //     return new PagedResult<T>
    //     {
    //         Items = items,
    //         TotalCount = await query.CountAsync(cancellationToken),
    //         HasMore = hasMore,
    //         NextCursor = hasMore
    //             ? orderByFunc(items.Last()).ToString()
    //             : null
    //     };

        // return new PagedResult<T>
        // {
        //     Items = items,
        //     TotalCount = await _dbSet.CountAsync(cancellationToken),
        //     HasMore = hasMore,
        //     NextCursor = hasMore
        //         ? items.Last() is null
        //             ? null
        //             : typeof(TKey)
        //                 .GetProperty(orderBy.Body.ToString().Split('.').Last())?
        //                 .GetValue(items.Last())?.ToString()
        //         : null
        // };
    //}


    // public virtual async Task<PagedResult<T>> GetPagedAsync<TKey>(Expression<Func<T, bool>>? predicate, Expression<Func<T, TKey>> orderBy, string? cursor, int pageSize, CancellationToken cancellationToken = default)
    // {
    //     var query = _dbSet.AsQueryable();

    //     if (predicate != null)
    //     {
    //         query = query.Where(predicate);
    //     }

    //     // Keyset pagination logic
    //     if (!string.IsNullOrEmpty(cursor))
    //     {
    //         var cursorValue = DecodeCursor<TKey>(cursor);
    //         var parameter = Expression.Parameter(typeof(T), "x");
    //         var property = Expression.Invoke(orderBy, parameter);
    //         var comparison = Expression.GreaterThan(property, Expression.Constant(cursorValue));
    //         var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
    //         query = query.Where(lambda);

    //     }

    //     query = query.OrderBy(orderBy);

    //     var items = await query.Take(pageSize + 1).ToListAsync(cancellationToken);

    //     var hasMore = items.Count > pageSize;

    //     if (hasMore)
    //     {
    //         items = items.Take(pageSize).ToList();
    //     }

    //     string? nextCursor = null;
    //     if (hasMore && items.Any())
    //     {
    //         var lastItem = items.Last();
    //         var lastKey = orderBy.Compile()(lastItem);
    //         nextCursor = EncodeCursor(lastKey);
    //     }

    //     var totalCount = await query.CountAsync(cancellationToken);

    //     return new PagedResult<T>
    //     {
    //         Items = items,
    //         NextCursor = nextCursor,
    //         TotalCount = totalCount,
    //         HasMore = hasMore
    //     };
    // }

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
