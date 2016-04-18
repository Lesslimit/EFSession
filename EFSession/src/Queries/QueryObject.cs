using System;
using System.Linq.Expressions;
using LinqKit;

namespace EFSession.Queries
{
    public abstract class QueryFilter<TEntity> : IQueryFilter<TEntity>
    {
        private Expression<Func<TEntity, bool>> finalExpression;

        public virtual Expression<Func<TEntity, bool>> Expression
        {
            get { return finalExpression; }
        }

        public Expression<Func<TEntity, bool>> And(Expression<Func<TEntity, bool>> query)
        {
            return finalExpression = finalExpression == null ? query : finalExpression.And(query.Expand());
        }

        public Expression<Func<TEntity, bool>> Or(Expression<Func<TEntity, bool>> query)
        {
            return finalExpression = finalExpression == null ? query : finalExpression.Or(query.Expand());
        }

        public Expression<Func<TEntity, bool>> And(IQueryFilter<TEntity> queryFilter)
        {
            return And(queryFilter.Expression);
        }

        public Expression<Func<TEntity, bool>> Or(IQueryFilter<TEntity> queryFilter)
        {
            return Or(queryFilter.Expression);
        }
    }
}