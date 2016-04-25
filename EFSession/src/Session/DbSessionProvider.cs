namespace EFSession.Session
{
    public class DbSessionProvider : IDbSessionProvider
    {
        public IDbSession OffspringFor(IDbSeedSession<IDbSession> seed, IDbContext dbContext)
        {
            return null;
        }
    }
}