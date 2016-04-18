using System;
using System.Linq;
using System.Linq.Expressions;

namespace EFSession.Queries
{
    public interface IQuery<T> : IOrderedQueryable<T>
    {
        IQuery<T> Where<TQFilter>(Func<TQFilter, TQFilter> queryFilter) where TQFilter : IQueryFilter<T>;

        IQuery<T> Where<TQFilter>(Func<TQFilter, Expression<Func<T, bool>>> queryFilter) where TQFilter : IQueryFilter<T>;
    }
}