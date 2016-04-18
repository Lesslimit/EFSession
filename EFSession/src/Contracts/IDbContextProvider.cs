using System.Data.Common;
using System.Data.Entity;

namespace EFSession
{
    public interface IDbContextProvider
    {
        DbContext ForSchema(string schema);
        DbContext ForDbName(string dbName);
        DbContext ForConnection(string schema, DbConnection connection);
    }
}