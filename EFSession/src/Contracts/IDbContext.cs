using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace EFSession
{
    public interface IDbContext : ICanPreserveConnection, IDisposable
    {
        DbContextConfiguration Configuration { get; }
        Database Database { get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbSet Set(Type entityType);

        DbEntityEntry Entry(object entity);
        ObjectContext GetObjectContext();

        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}