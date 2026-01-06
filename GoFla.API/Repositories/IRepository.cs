using System;
using System.Linq.Expressions;
using GoFla.API.Commons;

namespace GoFla.API.Repositories;

public interface IRepository<T> where T : class
{
Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync<TKey>(
    Expression<Func<T, bool>>? predicate,
    Expression<Func<T, TKey>> orderBy,
    string? cursor,
    int pageSize,
    bool descending,
    CancellationToken cancellationToken = default,
    params Expression<Func<T, object>>[] includes
       ) where TKey : struct, IComparable<TKey>;
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
