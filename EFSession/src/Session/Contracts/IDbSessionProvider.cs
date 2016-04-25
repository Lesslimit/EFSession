namespace EFSession.Session
{
    public interface IDbSessionProvider
    {
        IDbSession OffspringFor(IDbSeedSession<IDbSession> seed, IDbContext dbContext);
    }
}