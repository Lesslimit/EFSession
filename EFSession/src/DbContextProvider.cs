using System.Data.Common;
using System.Data.Entity;

namespace EFSession
{
    public class DbContextProvider<TDbContext> : IDbContextProvider where TDbContext : DbContext
    {
        #region Fields

        private readonly IDbContextFactory<TDbContext> dbContextFactory;
        private readonly ISqlServerConnectionStringBuilder connStringBuilder;
        private readonly IConfig config;

        #endregion Fields

        public DbContextProvider(IDbContextFactory<TDbContext> dbContextFactory,
                                 ISqlServerConnectionStringBuilder connStringBuilder,
                                 IConfig config)
        {
            this.dbContextFactory = dbContextFactory;
            this.connStringBuilder = connStringBuilder;
            this.config = config;
        }

        public DbContext ForSchema(string schema)
        {
            return dbContextFactory.Create(connStringBuilder.BuildFromSchema(schema), schema);
        }

        public DbContext ForDbName(string dbName)
        {
            return dbContextFactory.Create(connStringBuilder.BuildFromDbName(dbName), config.GetStringOrThrow("DefaultDbSchema"));
        }

        public DbContext ForConnection(string schema, DbConnection connection)
        {
            return dbContextFactory.Create(schema, connection, false);
        }
    }
}