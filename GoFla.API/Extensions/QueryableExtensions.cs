using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace GoFla.API.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> AppyCursorPagination<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey> >orderBy,
        TKey? cursor,
        int pageSize
    ) where TKey : IComparable<TKey>
    {
        // order first 
        query = query.OrderBy(orderBy);

        // Apply cursor if prsent
        if (cursor is not null)
        {
            var parameter = orderBy.Parameters[0];
            var body = Expression.GreaterThan(
                orderBy.Body,
                Expression.Constant(cursor)
            );

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            query = query.Where(lambda);
        }

         // Fetch one extra record to detect HasMore
         return query.Take(pageSize + 1);
    }
}
