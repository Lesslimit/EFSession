using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EFSession.DataStructures.Collections;
using EFSession.Extensions;
using EFSession.Schema.Parameters.Contracts;
using EFSession.Schema.Resolvers.Contracts;
using EFSession.StoredProcedures;

namespace EFSession.Session
{
    public class DbSessionManager : IDbSessionManager
    {
        private const string SystemDbName = "master";

        #region Fields

        private readonly IDependencyResolver dependencyResolver;
        private readonly IDbContextProvider dbContextProvider;
        private readonly IEnumerable<ISchemaResolver> schemaResolvers;
        private readonly IDbExecutionPolicy execPolicy;

        private readonly HashStack<Owned<ISeedSession<IDbSession>>> seedSessions =
            new HashStack<Owned<ISeedSession<IDbSession>>>();

        #endregion Fields

        #region Properties

        public bool HasAliveSessions
        {
            get
            {
                return seedSessions.Any(ss => ss.Value.Session.IsActive || ss.Value.Offsprings.Any(s => s.IsActive));
            }
        }

        #endregion Properties

        public DbSessionManager(IDependencyResolver dependencyResolver,
                                IDbContextProvider dbContextProvider,
                                IEnumerable<ISchemaResolver> schemaResolvers,
                                IDbExecutionPolicy execPolicy)
        {
            this.dependencyResolver = dependencyResolver;
            this.dbContextProvider = dbContextProvider;
            this.schemaResolvers = schemaResolvers;
            this.execPolicy = execPolicy;
        }

        public ISeedSession<IDbSession> Start(ISchemaCriteria schema, SessionHint sessionHint = SessionHint.None)
        {
            return Start(ResolveSchema(schema), sessionHint);
        }

        public ISeedSession<IDbSession> Start(string schema, SessionHint sessionHint = SessionHint.None)
        {
            if (!dependencyResolver.IsRegistered<ISeedSession<IDbSession>>())
            {
                throw new InvalidOperationException("Please register `IDbSession` with IoC");
            }

            var dbContext = sessionHint == SessionHint.SystemDb
                ? dbContextProvider.ForDbName(SystemDbName)
                : dbContextProvider.ForSchema(schema);

            var sqlParametersManager = dependencyResolver.Resolve<ISqlParametersManager>();
            var seedSession =
                dependencyResolver.Resolve<Owned<ISeedSession<IDbSession>>>(new NamedParameter("schema", schema),
                    new TypedParameter(typeof(ISqlParametersManager), sqlParametersManager),
                    new TypedParameter(typeof(IDependencyResolver), dependencyResolver),
                    new TypedParameter(typeof(DbContext), dbContext),
                    new TypedParameter(typeof(IDbExecutionPolicy), execPolicy));

            if (!seedSessions.Push(seedSession))
            {
                throw new InvalidOperationException("Seed Sessions collision");
            }

            seedSession.Value.Session.Begin(sessionHint);

            return seedSession.Value;
        }

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
            if (isDisposing)
            {
                Owned<ISeedSession<IDbSession>> seedSession;
                while (seedSessions.Any() && (seedSession = seedSessions.Pop()) != null)
                {
                    if (seedSession.Value.Session.IsActive)
                    {
                        seedSession.Dispose();
                    }
                }

                seedSessions.Clear();
                GC.SuppressFinalize(this);
            }
        }

        #endregion IDisposable

        #region Private Methods

        private string ResolveSchema(ISchemaCriteria schema)
        {
            foreach (var schemaResolver in schemaResolvers.Where(schemaResolver => schemaResolver.CanResolve(schema)))
            {
                return schemaResolver.Resolve(schema);
            }

            throw new InvalidOperationException($"Not resolver was able to handle {schema.GetType().Name}");
        }

        #endregion Private Methods
    }
}