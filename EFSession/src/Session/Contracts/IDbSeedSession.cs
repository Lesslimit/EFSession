using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EFSession.Queries;
using EFSession.StoredProcedures;
using IsolationLevel = System.Data.IsolationLevel;

namespace EFSession.Session
{
    public interface IDbSeedSession<out TSession> : IDisposable
        where TSession : IDbSession
    {
        TSession InnerSession { get; }

        IEnumerable<IDbSession> Offsprings { get; }

        bool HasActiveChildren { get; }

        IDbSession Offspring();

        IDatabaseQuery<TEntity> Query<TEntity>() where TEntity : class;

        TEntity AsInserted<TEntity>(TEntity entity) where TEntity : class;

        TEntity AsDeleted<TEntity>(TEntity entity) where TEntity : class;

        Task<int> SaveChangesAsync(bool inTransaction = true,
                                   IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
                                   CancellationToken cancellationToken = default(CancellationToken));

        TEntity Attach<TEntity>(TEntity entity) where TEntity : class;

        TEntity Detach<TEntity>(TEntity entity) where TEntity : class;

        bool IsProxy<TEntity>(TEntity entity) where TEntity : class;

        void SetTimeout(TimeSpan? timeout);

        Task SpNoResultAsync(Expression<Func<StoredProceduresContainer, string>> expressionToName, object parameters = null);

        ISpResult<TResult> Sp<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                       object parameters = null, params SqlParameter[] output);

        ISpResult<TResult> Sp<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName, TResult anonymousObj,
                                       object parameters = null, params SqlParameter[] output);

        ISpResult<TResult> SqlQuery<TResult>(string query, object parameters = null);

        Task SqlCommandAsync(string command, object parameters = null);

        void DisposeOffsprings(bool closeConnection = true);
    }
}