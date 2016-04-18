using System;
using System.Linq.Expressions;

namespace EFSession.Queries
{
    public interface IQueryFilter<TEntity>
    {
        Expression<Func<TEntity, bool>> Expression { get; }

        Expression<Func<TEntity, bool>> And(Expression<Func<TEntity, bool>> query);

        Expression<Func<TEntity, bool>> Or(Expression<Func<TEntity, bool>> query);

        Expression<Func<TEntity, bool>> And(IQueryFilter<TEntity> queryFilter);

        Expression<Func<TEntity, bool>> Or(IQueryFilter<TEntity> queryFilter);
    }
}