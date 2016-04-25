using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using EFSession.Emit.Contracts;
using EFSession.Extensions;
using EFSession.Queries;
using EFSession.StoredProcedures;
using IsolationLevel = System.Data.IsolationLevel;

// ReSharper disable InconsistentlySynchronizedField
namespace EFSession.Session
{
    public class DbSession : IDbSeedSession<IDbSession>, IDbSession
    {
        private readonly IDbContextProvider<IDbContext> dbContextProvider;
        private readonly ISqlParametersManager sqlParametersManager;

        private const string LogicalContextKey = "SuspendExecutionStrategy";

        #region Fields

        private readonly object locker = new object();

        private readonly IDymanicObjectCreator dymanicObjectCreator;
        private readonly IDbSeedSession<IDbSession> dbSeed;
        private readonly IDbContext dbContext;
        private readonly IDbExecutionPolicy execPolicy;
        private readonly IDbSessionProvider dbSessionProvider;
        private readonly IDbQueryProvider dbQueryProvider;

        private readonly ConcurrentQueue<KeyValuePair<IDbSession, IDbContext>> offsprings = new ConcurrentQueue<KeyValuePair<IDbSession, IDbContext>>();

        #endregion Fields

        #region Properties

        #region ISession

        public Guid Id { get; set; }

        public bool IsActive { get; private set; }

        #endregion ISession

        #region IDbSession

        public string Schema { get; }

        #endregion IDbSession

        #region ISeedSession<IDbSession>

        public IDbSession InnerSession => this;

        public IEnumerable<IDbSession> Offsprings
        {
            get { return offsprings.Select(kv => kv.Key); }
        }

        public bool HasActiveChildren
        {
            get { return offsprings.Any(cs => cs.Key.IsActive); }
        }

        #endregion ISeedSession<IDbSession>

        private bool IsSeed => dbSeed == null;

        public SessionHint Hint { get; private set; }

        private static bool SuspendExecutionStrategy
        {
            get { return (bool?)CallContext.LogicalGetData(LogicalContextKey) ?? false; }
            set { CallContext.LogicalSetData(LogicalContextKey, value); }
        }

        #endregion Properties

        #region Constructors

        public DbSession(string schema,
                         IDbContext dbContext,
                         ISqlParametersManager sqlParametersManager,
                         IDbContextProvider<IDbContext> dbContextProvider,
                         IDbExecutionPolicy execPolicy,
                         IDbQueryProvider dbQueryProvider,
                         IDbSessionProvider dbSessionProvider,
                         IDymanicObjectCreator dymanicObjectCreator)
            : this(schema, dbContext, sqlParametersManager, execPolicy, dbQueryProvider, dymanicObjectCreator, null)
        {
            this.dbContextProvider = dbContextProvider;
            this.dbSessionProvider = dbSessionProvider;
        }

        public DbSession(string schema,
                         IDbContext dbContext,
                         ISqlParametersManager sqlParametersManager,
                         IDbExecutionPolicy execPolicy,
                         IDbQueryProvider dbQueryProvider,
                         IDymanicObjectCreator dymanicObjectCreator,
                         IDbSeedSession<IDbSession> dbSeed)
        {
            if (dbSeed == null)
            {
                throw new ArgumentException($"DbContext for seed session has to implement {nameof(ICanPreserveConnection)}", nameof(dbContext));
            }

            this.Schema = schema;
            this.dbSeed = dbSeed;
            this.dbContext = dbContext;
            this.sqlParametersManager = sqlParametersManager;
            this.execPolicy = execPolicy;
            this.dbQueryProvider = dbQueryProvider;
            this.dymanicObjectCreator = dymanicObjectCreator;
        }

        #endregion Constructors

        #region IDbSession

        public IDbSession Begin(SessionHint sessionHint = SessionHint.None)
        {
            if (!IsActive)
            {
                Hint = sessionHint;
                IsActive = true;
            }

            return this;
        }

        #endregion IDbSession

        #region ISeedSession<IDbSession>

        public IDbSession Offspring()
        {
            var offspringContext = dbContextProvider.ForConnection(Schema, dbContext.Database.Connection);

            offspringContext.Configuration.EnsureTransactionsForFunctionsAndCommands = false;
            offspringContext.Database.CommandTimeout = dbContext.Database.CommandTimeout;

            var offspringSession = dbSessionProvider.OffspringFor(this, offspringContext);

            offsprings.Enqueue(new KeyValuePair<IDbSession, IDbContext>(offspringSession, offspringContext));

            dbContext.MarkConnectionAsPreserved();

            return offspringSession.Begin(Hint);
        }

        public IDatabaseQuery<TEntity> Query<TEntity>() where TEntity : class
        {
            var query = dbQueryProvider.For<TEntity>()
                                       .UsingContext(dbContext)
                                       .Create<TEntity>();

            if (Hint.HasFlag(SessionHint.NoTracking))
            {
                query.DoNotTrack();
                dbContext.Configuration.AutoDetectChangesEnabled = false;
            }

            if (!IsSeed)
            {
                execPolicy.Execute(() => dbContext.Database.Connection.EnsureConnectionOpened());
            }

            return query;
        }

        public void SetTimeout(TimeSpan? timeout)
        {
            dbContext.Database.CommandTimeout = timeout.HasValue ? new int?(Convert.ToInt32(timeout.Value.TotalSeconds)) : null;
        }

        public async Task SpNoResultAsync(Expression<Func<StoredProceduresContainer, string>> expressionToName, object parameters = null)
        {
            string spName;
            if (!expressionToName.TryGetName(out spName))
            {
                throw new ArgumentException(nameof(expressionToName));
            }

            await execPolicy.ExecuteAsync(async () =>
            {
                if (!IsSeed)
                {
                    await dbContext.Database.Connection.EnsureConnectionOpenedAsync()
                                                       .ConfigureAwait(false);
                }

                var sqlParameters = sqlParametersManager.PrepareParameters(parameters);

                // ReSharper disable once CoVariantArrayConversion
                await dbContext.Database
                               .ExecuteSqlCommandAsync($"EXEC {spName} {sqlParameters.ToSpParamsString()}", sqlParameters)
                               .ConfigureAwait(false);
            }).ConfigureAwait(false);

            DisposeSingleQueried();
        }

        public ISpResult<TResult> Sp<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName,
                                              object parameters = null, params SqlParameter[] output)
        {
            string spName;
            if (!expressionToName.TryGetName(out spName))
            {
                throw new ArgumentException(nameof(expressionToName));
            }

            var sqlParameters = sqlParametersManager.PrepareParameters(parameters, output);

            if (!IsSeed)
            {
                execPolicy.Execute(() => dbContext.Database.Connection.EnsureConnectionOpened());
            }

            // ReSharper disable once CoVariantArrayConversion
            var sqlQuery = dbContext.Database.SqlQuery<TResult>($"EXEC {spName} {sqlParameters.ToSpParamsString()}", sqlParameters);

            return new DbRawSqlQuerySpResult<TResult>(sqlQuery, execPolicy);
        }

        public ISpResult<TResult> Sp<TResult>(Expression<Func<StoredProceduresContainer, string>> expressionToName, TResult anonymousObj,
                                              object parameters = null, params SqlParameter[] output)
        {
            string spName;
            if (!expressionToName.TryGetName(out spName))
            {
                throw new ArgumentException(nameof(expressionToName));
            }

            var sqlParameters = sqlParametersManager.PrepareParameters(parameters, output);
            var objectConstructor = dymanicObjectCreator.CreateParametrizedConstructor(typeof(TResult).GetParametrizedConstructor());

            if (!IsSeed)
            {
                execPolicy.Execute(() => dbContext.Database.Connection.EnsureConnectionOpened());
            }

            var spResult = new AnonymousSpResult<TResult>(spName, dbContext.Database.Connection, sqlParameters,
                                                          objectConstructor, anonymousObj, execPolicy, IsSeed);

            return spResult;
        }

        public ISpResult<TResult> SqlQuery<TResult>(string command, object parameters = null)
        {
            return execPolicy.Execute(() =>
            {
                if (!IsSeed)
                {
                    dbContext.Database.Connection.EnsureConnectionOpened();
                }

                var sqlParameters = sqlParametersManager.PrepareParameters(parameters);

                // ReSharper disable once CoVariantArrayConversion
                return new DbRawSqlQuerySpResult<TResult>(dbContext.Database.SqlQuery<TResult>(command, sqlParameters), execPolicy);
            });
        }

        public async Task SqlCommandAsync(string command, object parameters = null)
        {
            await execPolicy.ExecuteAsync(async () =>
            {
                if (!IsSeed)
                {
                    await dbContext.Database.Connection.EnsureConnectionOpenedAsync()
                                                       .ConfigureAwait(false);
                }

                var sqlParameters = sqlParametersManager.PrepareParameters(parameters);

                // ReSharper disable once CoVariantArrayConversion
                await dbContext.Database.ExecuteSqlCommandAsync(command, sqlParameters)
                                        .ConfigureAwait(false);
            }).ConfigureAwait(false);

            DisposeSingleQueried();
        }

        public TEntity AsInserted<TEntity>(TEntity entity) where TEntity : class
        {
            lock (locker)
            {
                var entities = entity as IEnumerable;

                if (entities != null)
                {
                    var entitiesArray = entities as object[] ?? entities.Cast<object>().ToArray();
                    var entityType = entitiesArray.Select(e => e.GetType()).FirstOrDefault();

                    if (entityType != null)
                    {
                        dbContext.Set(entityType).AddRange(entitiesArray);
                    }
                }
                else
                {
                    dbContext.Set(entity.GetType()).Add(entity);
                }

                return entity;
            }
        }

        public TEntity AsDeleted<TEntity>(TEntity entity) where TEntity : class
        {
            lock (locker)
            {
                var entities = entity as IEnumerable;

                if (entities != null)
                {
                    var entitiesArray = entities as object[] ?? entities.Cast<object>().ToArray();
                    var entityType = entitiesArray.Select(e => e.GetType()).FirstOrDefault();

                    if (entityType != null)
                    {
                        dbContext.Set(entityType).RemoveRange(entitiesArray);
                    }
                }
                else
                {
                    dbContext.Set(entity.GetType()).Remove(entity);
                }

                return entity;
            }
        }

        public TEntity Attach<TEntity>(TEntity entity) where TEntity : class
        {
            lock (locker)
            {
                var dbSet = dbContext.Set<TEntity>();
                var entities = entity as IEnumerable;

                if (entities != null)
                {
                    var entitiesArray = entities as object[] ?? entities.Cast<object>().ToArray();
                    var entityType = entitiesArray.Select(e => e.GetType()).FirstOrDefault();

                    if (entityType != null)
                    {
                        var set = dbContext.Set(entityType);
                        foreach (var e in entitiesArray.Where(e => dbContext.Entry(e).State == EntityState.Detached))
                        {
                            set.Attach(e);
                        }
                    }
                }
                else
                {
                    dbSet.Attach(entity);
                }

                return entity;
            }
        }

        public TEntity Detach<TEntity>(TEntity entity) where TEntity : class
        {
            lock (locker)
            {
                var objectContext = dbContext.GetObjectContext();
                var entities = entity as IEnumerable;

                if (entities != null)
                {
                    var entitiesArray = entities as object[] ?? entities.Cast<object>().ToArray();

                    foreach (var e in entitiesArray)
                    {
                        foreach (var offspringSession in offsprings.Select(kv => kv.Key))
                        {
                            offspringSession.Detach(e);
                        }

                        objectContext.DetachSafely(e);
                    }
                }
                else
                {
                    foreach (var offspringSession in offsprings.Select(kv => kv.Key))
                    {
                        offspringSession.Detach(entity);
                    }

                    objectContext.DetachSafely(entity);
                }

                return entity;
            }
        }

        public bool IsProxy<TEntity>(TEntity entity) where TEntity : class
        {
            return entity != null && ObjectContext.GetObjectType(entity.GetType()) != entity.GetType();
        }

        public async Task<int> SaveChangesAsync(bool inTransaction = true,
                                                IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            var rowsAffected = 0;

            if (IsSeed)
            {
                if (!HasActiveChildren)
                {
                    rowsAffected = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                else if (inTransaction)
                {
                    await execPolicy.ExecuteAsync(async () =>
                    {
                        SuspendExecutionStrategy = true;
                        DbContextTransaction transaction = dbContext.Database.BeginTransaction(isolationLevel);

                        try
                        {
                            DbTransaction dbTransaction = transaction.UnderlyingTransaction;

                            rowsAffected += await dbContext.SaveChangesAsync().ConfigureAwait(false);

                            var offspringsChanges = await SaveOffspringsChangesAsync(dbTransaction).ConfigureAwait(false);

                            rowsAffected += offspringsChanges.RowsAffected;

                            transaction.Commit();
                        }
                        catch(Exception ex)
                        {
                            var dbUpdateException = ex as DbUpdateException;

                            var sqlException = dbUpdateException?.InnerException?.GetBaseException() as SqlException;

                            if (sqlException != null && sqlException.Errors.Cast<SqlError>().Any(sqlError => sqlError.Number == 1205) /* Deadlock */)
                            {
                                throw sqlException; //Transaction cannot be rollbacked when deadlock has happened
                            }

                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            transaction.Dispose();
                            SuspendExecutionStrategy = false;
                        }
                    }).ConfigureAwait(false);
                }
                else
                {
                    rowsAffected += await dbContext.SaveChangesAsync().ConfigureAwait(false);
                    var offspringsChanges = await SaveOffspringsChangesAsync(failOnFirstException: false).ConfigureAwait(false);

                    rowsAffected += offspringsChanges.RowsAffected;
                }
            }
            else
            {
                rowsAffected = await dbContext.SaveChangesAsync(cancellationToken)
                                              .ConfigureAwait(false);
            }

            if (Hint.HasFlag(SessionHint.DisposeOnSave))
            {
                Dispose(true);
            }

            return rowsAffected;
        }

        #endregion ISeedSession<IDbSession>

        #region IDisposable

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception ex) when (ex.IsHandleable())
            {
                //ignore (Dispose should not throw exceptions)
            }
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (IsSeed && !IsActive && HasActiveChildren)
            {
                throw new InvalidOperationException("Disposing Seed session with active children. State was corrupted. Do not dispose child sessions manually");
            }

            if (isDisposing && IsActive)
            {
                if (IsSeed)
                {
                    DisposeOffsprings();
                }

                dbContext.Dispose();

                IsActive = false;

            }
        }

        public void DisposeOffsprings(bool closeConnection = true)
        {
            KeyValuePair<IDbSession, IDbContext> offspring;
            while (offsprings.Any() && offsprings.TryDequeue(out offspring))
            {
                if (offspring.Key.IsActive)
                {
                    offspring.Key.Dispose();
                }
            }

            dbContext.ReleasePreservedConnection();

            if (closeConnection)
            {
                dbContext.Database.Connection.Close();
            }
        }

        #endregion IDisposable

        #region Private Stuff

        private async Task<OffspringSavingResult> SaveOffspringsChangesAsync(DbTransaction transaction = null, bool failOnFirstException = true)
        {
            OffspringSavingResult result;

            if (Hint.HasFlag(SessionHint.AtomicOffspings))
            {
                result = await SaveOffspringsConcurrentlyAsync(transaction, failOnFirstException).ConfigureAwait(false);
            }
            else
            {
                result = await SaveOffspingsSuccessivelyAsync(transaction, failOnFirstException).ConfigureAwait(false);
            }

            while (result.OffspringsBackupQueue.Any())
            {
                offsprings.Enqueue(result.OffspringsBackupQueue.Dequeue());
            }

            if (result.Exceptions.Any())
            {
                throw new AggregateException(result.Exceptions);
            }

            return result;
        }

        private async Task<OffspringSavingResult> SaveOffspingsSuccessivelyAsync(DbTransaction transaction, bool failOnFirstException)
        {
            var result = OffspringSavingResult.Empty();

            KeyValuePair<IDbSession, IDbContext> offspring;
            while (offsprings.Any() && offsprings.TryDequeue(out offspring))
            {
                if (!offspring.Key.IsActive)
                {
                    continue;
                }

                try
                {
                    var database = offspring.Value.Database;

                    if (transaction != null)
                    {
                        await database.Connection.EnsureConnectionOpenedAsync().ConfigureAwait(false);
                        database.UseTransaction(transaction);
                    }

                    result.OffspringsBackupQueue.Enqueue(offspring);
                    result.RowsAffected += await offspring.Value.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (failOnFirstException)
                    {
                        throw;
                    }

                    result.Exceptions.Add(exception);
                }
            }

            return result;
        }

        private async Task<OffspringSavingResult> SaveOffspringsConcurrentlyAsync(DbTransaction transaction, bool failOnFirstException)
        {
            int rowsAffected = 0;
            var cancellationTokenSource = new CancellationTokenSource();
            var saveChangesTasks = new List<Task>(offsprings.Count);
            var result = OffspringSavingResult.Empty();

            KeyValuePair<IDbSession, IDbContext> offspring;
            while (offsprings.Any() && offsprings.TryDequeue(out offspring))
            {
                if (!offspring.Key.IsActive)
                {
                    continue;
                }

                try
                {
                    var database = offspring.Value.Database;

                    if (transaction != null)
                    {
                        await database.Connection.EnsureConnectionOpenedAsync(cancellationToken: cancellationTokenSource.Token)
                                                 .ConfigureAwait(false);

                        database.UseTransaction(transaction);
                    }

                    result.OffspringsBackupQueue.Enqueue(offspring);

                    saveChangesTasks.Add(offspring.Value.SaveChangesAsync(cancellationTokenSource.Token)
                        .ContinueWith(t =>
                        {
                            if (!t.IsCanceled && !cancellationTokenSource.Token.IsCancellationRequested)
                            {
                                Interlocked.Add(ref rowsAffected, t.Result);
                            }
                        }, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled)
                        .ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                            {
                                if (failOnFirstException)
                                {
                                    cancellationTokenSource.Cancel(false);
                                }

                                result.Exceptions.Add(t.Exception);
                            }
                        }, TaskContinuationOptions.OnlyOnFaulted)
                        .ContinueWith(t =>
                        {
                            //TODO: Figure out why this is called
                        }, TaskContinuationOptions.OnlyOnCanceled));
                }
                catch (Exception exception)
                {
                    if (failOnFirstException)
                    {
                        throw;
                    }

                    result.Exceptions.Add(exception);
                }
            }

            await Task.WhenAll(saveChangesTasks).ConfigureAwait(false);

            result.RowsAffected = rowsAffected;

            return result;
        }

        private class OffspringSavingResult
        {
            public Queue<KeyValuePair<IDbSession, IDbContext>> OffspringsBackupQueue { get; }

            public ConcurrentBag<Exception> Exceptions { get; }

            public int RowsAffected { get; set; }

            private OffspringSavingResult()
            {
                OffspringsBackupQueue = new Queue<KeyValuePair<IDbSession, IDbContext>>();
                Exceptions = new ConcurrentBag<Exception>();
            }

            public static OffspringSavingResult Empty()
            {
                return new OffspringSavingResult();
            }
        }

        private void DisposeSingleQueried()
        {
            if (Hint.HasFlag(SessionHint.SingleQuery))
            {
                Dispose(true);
            }
        }

        #endregion Private Stuff
    }
}