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
    public class SqlServerPlainAdoSqlExecutor : ISqlExecutor
    {
        private readonly ISqlParametersManager sqlParametersManager;
        private readonly IDbExecutionPolicy execPolicy;

        private string connectionString;

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                // ReSharper disable once CollectionNeverQueried.Local
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(value)
                    {
                        AsynchronousProcessing = true
                    };

                if (string.IsNullOrEmpty(builder.ApplicationName) || builder.ApplicationName == ".Net SqlClient Data Provider")
                {
                    builder.ApplicationName = "Brandify";
                }

                connectionString = builder.ConnectionString;
            }
        }

        public SqlServerPlainAdoSqlExecutor(string connectionString, ISqlParametersManager sqlParametersManager,
                                            IDbExecutionPolicy execPolicy)
        {
            this.sqlParametersManager = sqlParametersManager;
            this.execPolicy = execPolicy;
            ConnectionString = connectionString;
        }

        public SqlServerPlainAdoSqlExecutor(IConfig config, ISqlParametersManager sqlParametersManager, IDbExecutionPolicy execPolicy)
            : this(config.GetStringOrThrow("DatabaseConnectionString"), sqlParametersManager, execPolicy)
        {
        }

        public IEnumerable<TResult> Sp<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                                Func<IDataRecord, int, TResult> converter, TimeSpan? timeout = null, params SqlParameter[] output)
        {
            return Sp(expressionToName, null, converter, timeout, output);
        }

        public IEnumerable<TResult> Sp<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                                object parameters, Func<IDataRecord, int, TResult> converter, TimeSpan? timeout = null, params SqlParameter[] output)
        {
            string spName;
            if (!expressionToName.TryGetName(out spName))
            {
                throw new ArgumentException(nameof(expressionToName));
            }

            return Sp(spName, parameters, converter, timeout, output);
        }

        public IEnumerable<TResult> Sp<TResult>(string spName, Func<IDataRecord, int, TResult> converter, TimeSpan? timeout = null, params SqlParameter[] output)
        {
            return Sp(spName, null, converter, timeout, output);
        }

        public IEnumerable<TResult> Sp<TResult>(string spName, object parameters, Func<IDataRecord, int, TResult> converter,
                                                TimeSpan? timeout = null, params SqlParameter[] output)
        {
            var parsedParameters = sqlParametersManager.PrepareParameters(parameters, output);

            if (timeout.HasValue)
            {
                SetTimeout(Convert.ToInt32(timeout.Value.TotalSeconds));
            }

            return execPolicy.Execute(() =>
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                using (var sqlCommand = new SqlCommand(spName, (sqlConnection)))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(parsedParameters);

                    sqlConnection.Open();
                    using (var dataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        IList<TResult> results = new List<TResult>();
                        while (dataReader.Read())
                        {
                            results.Add(converter(dataReader, results.Count));
                        }

                        return results;
                    }
                }
            });
        }

        public async Task<IEnumerable<TResult>> SpAsync<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                                           Func<IDataRecord, int, TResult> converter,
                                                           TimeSpan? timeout = null, CancellationToken cancellationToken = new CancellationToken(),
                                                           params SqlParameter[] output)
        {
            return await SpAsync(expressionToName, null, converter, timeout, cancellationToken, output);
        }

        public async Task<IEnumerable<TResult>> SpAsync<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                                           object parameters, Func<IDataRecord, int, TResult> converter,
                                                           TimeSpan? timeout = null, CancellationToken cancellationToken = new CancellationToken(),
                                                           params SqlParameter[] output)
        {
            string spName;
            if (!expressionToName.TryGetName(out spName))
            {
                throw new ArgumentException(nameof(expressionToName));
            }

            return await SpAsync(spName, parameters, converter, timeout, cancellationToken, output);
        }

        public async Task<IEnumerable<TResult>> SpAsync<TResult>(string spName, Func<IDataRecord, int, TResult> converter,
                                                           TimeSpan? timeout = null, CancellationToken cancellationToken = new CancellationToken(),
                                                           params SqlParameter[] output)
        {
            return await SpAsync(spName, null, converter, timeout, cancellationToken, output);
        }

        public async Task<IEnumerable<TResult>> SpAsync<TResult>(string spName, object parameters, Func<IDataRecord, int, TResult> converter,
                                                                 TimeSpan? timeout = null, CancellationToken cancellationToken = default(CancellationToken),
                                                                 params SqlParameter[] output)
        {
            var parsedParameters = sqlParametersManager.PrepareParameters(parameters, output);

            if (timeout.HasValue)
            {
                SetTimeout(Convert.ToInt32(timeout.Value.TotalSeconds));
            }

            return await execPolicy.ExecuteAsync<IEnumerable<TResult>>(async () =>
            {
                using (var sqlConnection = new SqlConnection(ConnectionString))
                using (var sqlCommand = new SqlCommand(spName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(parsedParameters);

                    await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    using (var dataReader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken)
                                                            .ConfigureAwait(false))
                    {
                        IList<TResult> results = new List<TResult>();
                        while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                        {
                            results.Add(converter(dataReader, results.Count));
                        }

                        return results;
                    }
                }
            });
        }

        private void SetTimeout(int seconds)
        {
            ConnectionString = new SqlConnectionStringBuilder(ConnectionString)
                {
                    ConnectTimeout = seconds
                }.ConnectionString;
        }
    }
}