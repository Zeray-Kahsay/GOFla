using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.EntityFrameworkCore;

namespace GoFla.API.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyCursorPagination<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> orderBy,
        TKey? cursor,
        int pageSize,
        bool descending
    ) where TKey : struct, IComparable<TKey>
    {
        // Order first
        query = descending
            ? query.OrderByDescending(orderBy)
            : query.OrderBy(orderBy);

        // Apply Cursor filter
        if (cursor.HasValue)
        {
            query = descending
                ? query.Where(e => EF.Property<TKey>(e!, ((MemberExpression)orderBy.Body).Member.Name)
                               .CompareTo(cursor.Value) < 0)
                : query.Where(e => EF.Property<TKey>(e!, ((MemberExpression)orderBy.Body).Member.Name)
                               .CompareTo(cursor.Value) > 0);
        }


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
