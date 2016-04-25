using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EFSession.Queries;
using EFSession.StoredProcedures;

namespace EFSession.Session
{
    public interface IDbSession : ISession, IDisposable
    {
        string Schema { get; }

        SessionHint Hint { get; }

        IDbSession Begin(SessionHint sessionHint = SessionHint.None);

        IDatabaseQuery<TEntity> Query<TEntity>() where TEntity : class;

        Task SpNoResultAsync(Expression<Func<StoredProceduresContainer, string>> expressionToName, object parameters = null);

        ISpResult<TResult> Sp<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                       object parameters = null, params SqlParameter[] output);

        ISpResult<TResult> Sp<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName, TResult anonymousObj,
                                       object parameters = null, params SqlParameter[] output);

        ISpResult<TResult> SqlQuery<TResult>(string query, object parameters = null);

        Task SqlCommandAsync(string query, object parameters = null);

        TEntity AsInserted<TEntity>(TEntity entity) where TEntity : class;

        TEntity AsDeleted<TEntity>(TEntity entity) where TEntity : class;

        TEntity Attach<TEntity>(TEntity entity) where TEntity : class;

        TEntity Detach<TEntity>(TEntity entity) where TEntity : class;

        void SetTimeout(TimeSpan? timeout);
    }
}