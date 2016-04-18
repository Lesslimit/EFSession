using System.Data.Common;
using System.Data.Entity;

namespace EFSession
{
    public interface IDbContextFactory<out TContext> where TContext : DbContext
    {
        TContext Create(string connectionString, string schema);
        TContext Create(string schema, DbConnection connection, bool ownsConnection);
    }
}