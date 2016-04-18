using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace EFSession.StoredProcedures
{
    public class DbRawSqlQuerySpResult<T> : ISpResult<T>
    {
        private readonly DbRawSqlQuery<T> dbRawSqlQuery;

        public DbRawSqlQuerySpResult(DbRawSqlQuery<T> dbRawSqlQuery)
        {
            this.dbRawSqlQuery = dbRawSqlQuery;
        }

        #region Single*Async

        public Task<T> SingleAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.SingleAsync(cancellationToken);
        }

        public  Task<T> SingleAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.SingleAsync(predicate, cancellationToken);
        }

        public Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.SingleAsync(cancellationToken);
        }

        public Task<T> SingleOrDefaultAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        #endregion

        #region First*Async

        public async Task<T> FirstAsync(CancellationToken cancellationToken)
        {
            return await dbRawSqlQuery.FirstAsync(cancellationToken);
        }

        public Task<T> FirstAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.FirstAsync(predicate, cancellationToken);
        }

        public Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.FirstOrDefaultAsync(cancellationToken);
        }

        public Task<T> FirstOrDefaultAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        #endregion

        public async Task<List<T>> ToListAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dbRawSqlQuery.ToListAsync(cancellationToken);
        }

        public Task<Dictionary<TKey, T>> ToDictionaryAsync<TKey>(Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer = null,
                                                                 CancellationToken cancellationToken = default(CancellationToken))
        {
            return ToDictionaryAsync(keySelector, t => t, comparer, cancellationToken);
        }

        public async Task<Dictionary<TKey, TResult>> ToDictionaryAsync<TKey, TResult>(Func<T, TKey> keySelector, Func<T, TResult> elementSelector, IEqualityComparer<TKey> comparer = null,
                                                                                      CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dbRawSqlQuery.ToDictionaryAsync(keySelector, elementSelector, comparer, cancellationToken);
        }

        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dbRawSqlQuery.AnyAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dbRawSqlQuery.AnyAsync(predicate, cancellationToken);
        }

        public Task<bool> AllAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.AllAsync(predicate, cancellationToken);
        }

        public Task<bool> ContainsAsync(T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.ContainsAsync(value, cancellationToken);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.CountAsync(cancellationToken);
        }

        public Task<T> MaxAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.MaxAsync(cancellationToken);
        }

        public Task<T> MinAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.MinAsync(cancellationToken);
        }

        public Task<int> CountAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.CountAsync(predicate, cancellationToken);
        }

        public Task ForEachAsync(Action<T> action, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbRawSqlQuery.ForEachAsync(action, cancellationToken);
        }
    }
}