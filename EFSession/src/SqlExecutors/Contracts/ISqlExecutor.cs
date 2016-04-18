using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EFSession.StoredProcedures;

namespace EFSession.SqlExecutors
{
    public interface ISqlExecutor
    {
        string ConnectionString { get; }

        IEnumerable<TResult> Sp<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                         Func<IDataRecord, int, TResult> converter, TimeSpan? timeout = null, params SqlParameter[] output);

        IEnumerable<TResult> Sp<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                         object parameters, Func<IDataRecord, int, TResult> converter, TimeSpan? timeout = null, params SqlParameter[] output);

        IEnumerable<TResult> Sp<TResult>(string spName, Func<IDataRecord, int, TResult> converter, TimeSpan? timeout = null, params SqlParameter[] output);

        IEnumerable<TResult> Sp<TResult>(string spName, object parameters, Func<IDataRecord, int, TResult> converter,
                                         TimeSpan ? timeout = null, params SqlParameter[] output);

        Task<IEnumerable<TResult>> SpAsync<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                                    Func<IDataRecord, int, TResult> converter, TimeSpan? timeout = null,
                                                    CancellationToken cancellationToken = new CancellationToken(), params SqlParameter[] output);

        Task<IEnumerable<TResult>> SpAsync<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                                    object parameters, Func<IDataRecord, int, TResult> converter,
                                                    TimeSpan? timeout = null, CancellationToken cancellationToken = new CancellationToken(), params SqlParameter[] output);

        Task<IEnumerable<TResult>> SpAsync<TResult>(string spName, object parameters, Func<IDataRecord, int, TResult> converter,
                                                    TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken), params SqlParameter[] output);

        Task<IEnumerable<TResult>> SpAsync<TResult>(string spName, Func<IDataRecord, int, TResult> converter,
                                                    TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken), params SqlParameter[] output);
    }
}