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
        int pageSize,
        bool descending
    ) where TKey : struct, IComparable<TKey>
    {
        if (cursor.HasValue)
        {
            query = descending
                ? query.Where(BuildComparison(orderBy, cursor.Value, lessThan: true))
                : query.Where(BuildComparison(orderBy, cursor.Value, lessThan: false));
        }

        query = descending
            ? query.OrderByDescending(orderBy)
            : query.OrderBy(orderBy);
        // order first 
        //query = query.OrderBy(orderBy);

        // Apply cursor if prsent
        // if (cursor is not null)
        // {
        //     var parameter = orderBy.Parameters[0];
        //     var body = Expression.GreaterThan(
        //         orderBy.Body,
        //         Expression.Constant(cursor)
        //     );

        //     var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
        //     query = query.Where(lambda);
        // }

        // Fetch one extra record to detect HasMore
        return query.Take(pageSize + 1);
    }

    private static Expression<Func<T, bool>> BuildComparison<T, TKey>(
    Expression<Func<T, TKey>> keySelector,
    TKey cursor,
    bool lessThan
)
    {
        var param = keySelector.Parameters[0];
        var left = keySelector.Body;
        var right = Expression.Constant(cursor);

        var comparison = lessThan
            ? Expression.LessThan(left, right)
            : Expression.GreaterThan(left, right);

        return Expression.Lambda<Func<T, bool>>(comparison, param);
    }

}
