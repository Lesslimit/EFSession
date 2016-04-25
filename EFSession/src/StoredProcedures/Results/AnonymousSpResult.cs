using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFSession.Emit.Contracts;
using EFSession.Extensions;
using EFSession.Session;

namespace EFSession.StoredProcedures
{
    public class AnonymousSpResult<T> : ISpResult<T>
    {
        private readonly ObjectConstructor<object> objectConstructor;
        private readonly object anonymousObj;
        private readonly IDbExecutionPolicy execPolicy;
        private readonly bool closeConnection;
        private readonly string spName;
        private readonly DbConnection connection;
        private readonly IEnumerable<SqlParameter> sqlParameters;

        public AnonymousSpResult(string spName, DbConnection connection, IEnumerable<SqlParameter> sqlParameters,
                                 ObjectConstructor<object> objectConstructor, object anonymousObj,
                                 IDbExecutionPolicy execPolicy, bool closeConnection)
        {
            this.spName = spName;
            this.connection = connection;
            this.sqlParameters = sqlParameters;
            this.objectConstructor = objectConstructor;
            this.anonymousObj = anonymousObj;
            this.execPolicy = execPolicy;
            this.closeConnection = closeConnection;
        }

        #region Single*Async

        public async Task<T> SingleAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken))
                {
                    if (!dataReader.HasRows)
                    {
                        throw new InvalidOperationException("The input sequence is empty.");
                    }

                    var rowValues = new object[propertyDescriptorCollection.Count];

                    await dataReader.ReadAsync(cancellationToken);

                    for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                    {
                        rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                    }

                    if (await dataReader.ReadAsync(cancellationToken))
                    {
                        throw new InvalidOperationException("The input sequence contains more than one element.");
                    }

                    return (T)objectConstructor(rowValues);
                }
            });
        }

        public async Task<T> SingleAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken))
                {
                    if (!dataReader.HasRows)
                    {
                        throw new InvalidOperationException("The input sequence is empty.");
                    }

                    var rowValues = new object[propertyDescriptorCollection.Count];
                    T result = default(T);
                    bool hasResult = false;

                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            PropertyDescriptor propertyDesc = propertyDescriptorCollection[i];
                            object value = dataReader[propertyDesc.Name];

                            rowValues[i] = value is DBNull
                                ? propertyDesc.ComponentType.GetDefaultValuePerf()
                                : value;
                        }

                        var resultObj = (T)objectConstructor(rowValues);

                        if (predicate(resultObj) && !hasResult)
                        {
                            result = resultObj;
                            hasResult = true;
                        }
                        else
                        {
                            throw new InvalidOperationException("The input sequence contains more than one element.");
                        }
                    }

                    if (hasResult)
                    {
                        return result;
                    }

                    throw new InvalidOperationException("No element satisfies the condition in predicate.");
                }
            });
        }

        public async Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken))
                {
                    if (!dataReader.HasRows)
                    {
                        return Default(propertyDescriptorCollection);
                    }

                    var rowValues = new object[propertyDescriptorCollection.Count];
                    T result = default(T);
                    bool hasResult = false;

                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            PropertyDescriptor propertyDesc = propertyDescriptorCollection[i];
                            object value = dataReader[propertyDesc.Name];

                            rowValues[i] = value is DBNull
                                ? propertyDesc.ComponentType.GetDefaultValuePerf()
                                : value;
                        }

                        var resultObj = (T)objectConstructor(rowValues);

                        if (!hasResult)
                        {
                            result = resultObj;
                            hasResult = true;
                        }
                        else
                        {
                            throw new InvalidOperationException("The input sequence contains more than one element.");
                        }
                    }

                    return hasResult ? result : Default(propertyDescriptorCollection);
                }
            });
        }

        public async Task<T> SingleOrDefaultAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken))
                {
                    if (!dataReader.HasRows)
                    {
                        return Default(propertyDescriptorCollection);
                    }

                    var rowValues = new object[propertyDescriptorCollection.Count];
                    T result = default(T);
                    bool hasResult = false;

                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            PropertyDescriptor propertyDesc = propertyDescriptorCollection[i];
                            object value = dataReader[propertyDesc.Name];

                            rowValues[i] = value is DBNull
                                ? propertyDesc.ComponentType.GetDefaultValuePerf()
                                : value;
                        }

                        var resultObj = (T)objectConstructor(rowValues);

                        if (predicate(resultObj) && !hasResult)
                        {
                            result = resultObj;
                            hasResult = true;
                        }
                        else
                        {
                            throw new InvalidOperationException("More than one element satisfies the condition in predicate.");
                        }
                    }

                    return hasResult ? result : Default(propertyDescriptorCollection);
                }
            });
        }

        #endregion

        #region First*Async

        public async Task<T> FirstAsync(CancellationToken cancellationToken)
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken))
                {
                    if (!dataReader.HasRows)
                    {
                        throw new InvalidOperationException("The input sequence is empty.");
                    }

                    var rowValues = new object[propertyDescriptorCollection.Count];

                    await dataReader.ReadAsync(cancellationToken);

                    for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                    {
                        rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                    }

                    return (T)objectConstructor(rowValues);
                }
            });
        }

        public async Task<T> FirstAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken))
                {
                    if (!dataReader.HasRows)
                    {
                        throw new InvalidOperationException("The input sequence is empty.");
                    }

                    var rowValues = new object[propertyDescriptorCollection.Count];

                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                        }

                        var result = (T)objectConstructor(rowValues);

                        if (predicate(result))
                        {
                            return result;
                        }
                    }

                    throw new InvalidOperationException("No element satisfies the condition in predicate.");
                }
            });
        }

        public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken))
                {
                    if (!dataReader.HasRows)
                    {
                        return Default(propertyDescriptorCollection);
                    }

                    var rowValues = new object[propertyDescriptorCollection.Count];

                    await dataReader.ReadAsync(cancellationToken);

                    for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                    {
                        rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                    }

                    return (T)objectConstructor(rowValues);
                }
            });
        }

        public async Task<T> FirstOrDefaultAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken))
                {
                    if (!dataReader.HasRows)
                    {
                        return Default(propertyDescriptorCollection);
                    }

                    var rowValues = new object[propertyDescriptorCollection.Count];

                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                        }

                        var result = (T)objectConstructor(rowValues);

                        if (predicate(result))
                        {
                            return result;
                        }
                    }

                    return Default(propertyDescriptorCollection);
                }
            });
        }

        #endregion

        public async Task<List<T>> ToListAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken)
                                                                        .ConfigureAwait(false))
                {
                    var result = new List<T>();

                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var rowValues = new object[propertyDescriptorCollection.Count];

                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                        }

                        result.Add((T)objectConstructor(rowValues));
                    }

                    return result;
                }
            }).ConfigureAwait(false);
        }

        public async Task<Dictionary<TKey, T>> ToDictionaryAsync<TKey>(Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer = null,
                                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ToDictionaryAsync(keySelector, t => t, comparer, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Dictionary<TKey, TResult>> ToDictionaryAsync<TKey, TResult>(Func<T, TKey> keySelector, Func<T, TResult> elementSelector, IEqualityComparer<TKey> comparer = null,
                                                                                      CancellationToken cancellationToken = default(CancellationToken))
        {
            comparer = comparer ?? EqualityComparer<TKey>.Default;

            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken)
                                                          .ConfigureAwait(false))
                {
                    var result = new Dictionary<TKey, TResult>(comparer);

                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var rowValues = new object[propertyDescriptorCollection.Count];

                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            var property = propertyDescriptorCollection[i];
                            var value = dataReader[property.Name];

                            rowValues[i] = value is DBNull ? property.ComponentType.GetDefaultValuePerf() : value;
                        }

                        var resultObj = (T)objectConstructor(rowValues);
                        result.Add(keySelector(resultObj), elementSelector(resultObj));
                    }

                    return result;
                }
            }).ConfigureAwait(false);
        }

        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.SingleRow, cancellationToken)
                                                                                                 .ConfigureAwait(false))
                {
                    return dataReader.HasRows;
                }
            }).ConfigureAwait(false);
        }

        public async Task<bool> AnyAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken)
                                                                                               .ConfigureAwait(false))
                {
                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var rowValues = new object[propertyDescriptorCollection.Count];

                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                        }

                        if (predicate((T)objectConstructor(rowValues)))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }).ConfigureAwait(false);
        }

        public async Task<bool> AllAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken)
                                                                                                .ConfigureAwait(false))
                {
                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var rowValues = new object[propertyDescriptorCollection.Count];

                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                        }

                        if (!predicate((T)objectConstructor(rowValues)))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }).ConfigureAwait(false);
        }

        public async Task<bool> ContainsAsync(T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken)
                                                                                               .ConfigureAwait(false))
                {
                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var rowValues = new object[propertyDescriptorCollection.Count];

                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                        }

                        if (value.Equals((T)objectConstructor(rowValues)))
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }).ConfigureAwait(false);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.KeyInfo, cancellationToken)
                                                                                               .ConfigureAwait(false))
                {
                    int count = 0;
                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        count++;
                    }

                    return count;
                }
            }).ConfigureAwait(false);
        }

        public async Task<int> CountAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken)
                                                                                               .ConfigureAwait(false))
                {
                    int count = 0;
                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var rowValues = new object[propertyDescriptorCollection.Count];

                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                        }

                        if (predicate((T)objectConstructor(rowValues)))
                        {
                            count++;
                        }
                    }

                    return count;
                }
            }).ConfigureAwait(false);
        }

        public async Task ForEachAsync(Action<T> action, CancellationToken cancellationToken = default(CancellationToken))
        {
            await execPolicy.ExecuteAsync(async () =>
            {
                PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(anonymousObj);

                using (DbDataReader dataReader = await ExecuteReaderAsync(CommandBehavior.Default, cancellationToken)
                                                                                               .ConfigureAwait(false))
                {
                    while (await dataReader.ReadAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var rowValues = new object[propertyDescriptorCollection.Count];

                        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
                        {
                            rowValues[i] = GetFieldValueOrDefault(propertyDescriptorCollection[i], dataReader);
                        }

                        action((T)objectConstructor(rowValues));
                    }
                }
            }).ConfigureAwait(false);
        }

        #region Private Stuff

        private async Task<DbDataReader> ExecuteReaderAsync(CommandBehavior commandBehavior = CommandBehavior.CloseConnection,
                                                            CancellationToken cancellationToken = default(CancellationToken))
        {
            DbCommand sqlCommand = connection.CreateCommand();

            sqlCommand.Parameters.AddRange(sqlParameters.ToArray());
            sqlCommand.CommandText = spName;
            sqlCommand.CommandType = CommandType.StoredProcedure;

            if (connection.State == ConnectionState.Closed)
            {
                await connection.EnsureConnectionOpenedAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            if (closeConnection)
            {
                commandBehavior |= CommandBehavior.CloseConnection;
            }

            return await sqlCommand.ExecuteReaderAsync(commandBehavior, cancellationToken).ConfigureAwait(false);
        }

        private T Default(PropertyDescriptorCollection propertyDescriptorCollection)
        {
            var args = new object[propertyDescriptorCollection.Count];

            for (int i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                args[i] = propertyDescriptorCollection[i].ComponentType.GetDefaultValuePerf();
            }

            return (T)objectConstructor(args);
        }

        private static object GetFieldValueOrDefault(PropertyDescriptor propertyDesc, IDataRecord dataReader)
        {
            object value = dataReader[propertyDesc.Name];

            return value is DBNull
                ? propertyDesc.ComponentType.GetDefaultValuePerf()
                : value;
        }

        #endregion
    }
}