using EFSession.Queries;

namespace EFSession.Session
{
    public interface IDbQueryProvider
    {
        IDbQueryProvider For<T>();
        IDbQueryProvider UsingContext(IDbContext dbContext);
        IDatabaseQuery<T> Create<T>() where T : class;
    }
}