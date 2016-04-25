using System.Data.Entity.Infrastructure;

namespace EFSession.Queries
{
    public interface IDatabaseQuery<TEntity> : IQuery<TEntity>, IDbAsyncEnumerable<TEntity>
        where TEntity : class
    {
        IQuery<TEntity> ForId<TId>(TId id);

        IQuery<TEntity> DoNotTrack();
    }
}