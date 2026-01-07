using System;
using System.Linq.Expressions;
using GoFla.API.Commons;
using GoFla.API.Domain;

namespace GoFla.API.Repositories;

public interface IRestaurantRepository : IRepository<Restaurant>
{
    Task<Restaurant?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<Restaurant>> GetPagedWithDetailsAsync(
        Expression<Func<Restaurant, bool>> predicate,
        Expression<Func<Restaurant, int>> orderBy,
        string? cursor,
        int pageSize,
        CancellationToken cancellationToke = default
    );
    Task<List<Restaurant>> GetByOwnerAsync(string ownerId, CancellationToken cancellationToken = default);
}
