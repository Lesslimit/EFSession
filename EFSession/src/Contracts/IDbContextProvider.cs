using System.Data.Common;

namespace EFSession
{
    public interface IDbContextProvider<out TDbContext> where TDbContext : IDbContext
    {
        TDbContext ForSchema(string schema);
        TDbContext ForDbName(string dbName);
        TDbContext ForConnection(string schema, DbConnection connection);
    }
}