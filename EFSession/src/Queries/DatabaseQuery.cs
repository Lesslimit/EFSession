using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using EFSession.Extensions;
using LinqKit;

namespace EFSession.Queries
{
    public interface IDatabaseQuery<TEntity> : IQuery<TEntity>, IDbAsyncEnumerable<TEntity>
        where TEntity : class
    {
        IQuery<TEntity> ForId<TId>(TId id);

        IQuery<TEntity> DoNotTrack();
    }

    public class DatabaseQuery<TEntity> : IDatabaseQuery<TEntity> where TEntity : class
    {
        #region Fields

        private IDbSet<TEntity> dbSet;
        private IQueryable<TEntity> queryable;
        private readonly DbContext dbContext;
        private readonly IDependencyResolver dependencyResolver;

        #endregion Fields

        #region Properties

        #region IQuery<TEntity>

        public Expression Expression => Queryable.Expression;

        public Type ElementType => Queryable.ElementType;

        public IQueryProvider Provider => Queryable.Provider;

        #endregion IQuery<TEntity>

        private IDbSet<TEntity> DbSet => dbSet ?? (dbSet = dbContext.Set<TEntity>());

        private IQueryable<TEntity> Queryable
        {
            get { return queryable ?? DbSet.AsQueryable(); }
            set { queryable = value; }
        }

        #endregion Properties

        public DatabaseQuery(DbContext dbContext, IDependencyResolver dependencyResolver)
        {
            this.dbContext = dbContext;
            this.dependencyResolver = dependencyResolver;
        }

        #region IQuery<TEntity>

        public IQuery<TEntity> Where<TQFilter>(Func<TQFilter, TQFilter> queryFilter)
            where TQFilter : IQueryFilter<TEntity>
        {
            Queryable = Queryable.AsExpandable()
                                 .Where(queryFilter(dependencyResolver.Resolve<TQFilter>()).Expression);

            return this;
        }

        public IQuery<TEntity> Where<TQFilter>(Func<TQFilter, Expression<Func<TEntity, bool>>> filterExpression)
            where TQFilter : IQueryFilter<TEntity>
        {
            Queryable = Queryable.AsExpandable()
                                 .Where(filterExpression(dependencyResolver.Resolve<TQFilter>()));

            return this;
        }

        public IQuery<TEntity> ForId<TId>(TId id)
        {
            var keys = dbContext.GetObjectContext().CreateObjectSet<TEntity>().EntitySet.ElementType.KeyMembers;

            if (!keys.Any())
            {
                throw new InvalidOperationException("Entity has no key property");
            }

            if (keys.Count > 1)
            {
                throw new InvalidOperationException("Entity has more than one key property");
            }

            Queryable = Queryable.Where(ComposePropertyEqualsExpression(id, keys[0].Name));

            return this;
        }

        public IQuery<TEntity> DoNotTrack()
        {
            Queryable = Queryable.AsNoTracking();

            return this;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return DbSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IQuery<TEntity>

        #region IDbAsyncEnumerable<TEntity>

        public IDbAsyncEnumerator<TEntity> GetAsyncEnumerator()
        {
            return ((IDbAsyncEnumerable<TEntity>)Queryable).GetAsyncEnumerator();
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        #endregion IDbAsyncEnumerable<TEntity>

        #region Private

        private static Expression<Func<TEntity, bool>> ComposePropertyEqualsExpression<TId>(TId id, string propertyName)
        {
            var entityType = typeof(TEntity);

            var parameterExpression = Expression.Parameter(entityType);
            var memberExpression = Expression.Property(parameterExpression, propertyName);
            var binaryExpression = Expression.Equal(memberExpression, Expression.Constant(id));
            var expression = Expression.Lambda<Func<TEntity, bool>>(binaryExpression, parameterExpression);

            return expression;
        }

        #endregion Private
    }
}