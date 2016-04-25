namespace EFSession.Queries
{
    public interface IQueryFilterProvider
    {
        TQFilter Get<TEntity, TQFilter>() where TQFilter : IQueryFilter<TEntity>;
    }
}