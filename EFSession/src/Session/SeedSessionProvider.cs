namespace EFSession.Session
{
    public class SeedSessionProvider : ISeedSessionProvider
    {
        public ISeedSessionProvider ForSchema(string schema)
        {
            return null;
        }

        public T Resolve<T>(IDbContext dbContext)
        {
            return default(T);
        }
    }
}