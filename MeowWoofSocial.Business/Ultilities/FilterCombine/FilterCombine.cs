using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Ultilities.FilterCombine
{
    public static class FilterCombine
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));
            var body = Expression.AndAlso(
                Expression.Invoke(first, parameter),
                Expression.Invoke(second, parameter)
            );
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Func<IQueryable<T>, IOrderedQueryable<T>> AndThen<T>(
        this Func<IQueryable<T>, IOrderedQueryable<T>> first,
        Func<IQueryable<T>, IOrderedQueryable<T>> second)
        {
            return query => second(first(query));
        }
    }
}
