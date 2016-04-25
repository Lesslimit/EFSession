namespace EFSession.Session
{
    public interface ISeedSessionProvider
    {
        ISeedSessionProvider ForSchema(string schema);
        T Resolve<T>(IDbContext dbContext);
    }
}