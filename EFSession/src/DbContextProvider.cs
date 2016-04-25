using System.Data.Common;
using System.Data.Entity;

namespace EFSession
{
    public class DbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : IDbContext
    {
        #region Fields

        private readonly IDbContextFactory<TDbContext> dbContextFactory;
        private readonly ISqlServerConnectionStringBuilder connStringBuilder;

        #endregion Fields

        public DbContextProvider(IDbContextFactory<TDbContext> dbContextFactory,
                                 ISqlServerConnectionStringBuilder connStringBuilder)
        {
            this.dbContextFactory = dbContextFactory;
            this.connStringBuilder = connStringBuilder;
        }

        public TDbContext ForSchema(string schema)
        {
            return dbContextFactory.Create(connStringBuilder.BuildFromSchema(schema), schema);
        }

        public TDbContext ForDbName(string dbName)
        {
            return dbContextFactory.Create(connStringBuilder.BuildFromDbName(dbName), "dbo");
        }

        public TDbContext ForConnection(string schema, DbConnection connection)
        {
            return dbContextFactory.Create(schema, connection, false);
        }
    }
}