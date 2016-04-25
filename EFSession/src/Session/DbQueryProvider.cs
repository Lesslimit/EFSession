using EFSession.Queries;

namespace EFSession.Session
{
    public class DbQueryProvider : IDbQueryProvider
    {
        public IDbQueryProvider For<T>()
        {
            return null;
        }

        public IDbQueryProvider UsingContext(IDbContext dbContext)
        {
            return null;
        }

        public IDatabaseQuery<T> Create<T>() where T : class
        {
            return null;
        }
    }
}