using System;
using System.Collections.Generic;
using System.Linq;
using EFSession.DataStructures.Collections;
using EFSession.Extensions;
using EFSession.Schema.Parameters.Contracts;
using EFSession.Schema.Resolvers.Contracts;

namespace EFSession.Session
{
    public class DbSessionManager : IDbSessionManager
    {
        private const string SystemDbName = "master";

        #region Fields

        private readonly IDbContextProvider<IDbContext> dbContextProvider;
        private readonly IEnumerable<ISchemaResolver> schemaResolvers;
        private readonly ISeedSessionProvider seedSessionProvider;

        private readonly HashStack<IDbSeedSession<IDbSession>> seedSessions = new HashStack<IDbSeedSession<IDbSession>>();

        #endregion Fields

        #region Properties

        public bool HasAliveSessions
        {
            get
            {
                return seedSessions.Any(ss => ss.InnerSession.IsActive || ss.Offsprings.Any(s => s.IsActive));
            }
        }

        #endregion Properties

        public DbSessionManager(IDbContextProvider<IDbContext> dbContextProvider,
                                ISeedSessionProvider seedSessionProvider,
                                IEnumerable<ISchemaResolver> schemaResolvers)
        {
            this.dbContextProvider = dbContextProvider;
            this.schemaResolvers = schemaResolvers;
            this.seedSessionProvider = seedSessionProvider;
        }

        public IDbSeedSession<IDbSession> Start(ISchemaCriteria schema, SessionHint sessionHint = SessionHint.None)
        {
            return Start(ResolveSchema(schema), sessionHint);
        }

        public IDbSeedSession<IDbSession> Start(string schema, SessionHint sessionHint = SessionHint.None)
        {
            var dbContext = sessionHint == SessionHint.SystemDb
                ? dbContextProvider.ForDbName(SystemDbName)
                : dbContextProvider.ForSchema(schema);

            var seedSession = seedSessionProvider.ForSchema(schema)
                                                 .Resolve<IDbSeedSession<IDbSession>>(dbContext);

            if (!seedSessions.Push(seedSession))
            {
                throw new InvalidOperationException("Seed Sessions collision");
            }

            seedSession.InnerSession.Begin(sessionHint);

            return seedSession;
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
                IDbSeedSession<IDbSession> seedSession;
                while (seedSessions.Any() && (seedSession = seedSessions.Pop()) != null)
                {
                    if (seedSession.InnerSession.IsActive)
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