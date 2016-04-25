using System.Data.Common;

namespace EFSession
{
    public interface IDbContextFactory<out TContext> where TContext : IDbContext
    {
        TContext Create(string connectionString, string schema);
        TContext Create(string schema, DbConnection connection, bool ownsConnection);
    }
}