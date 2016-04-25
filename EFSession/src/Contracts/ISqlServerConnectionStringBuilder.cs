namespace EFSession
{
    public interface ISqlServerConnectionStringBuilder : IConnectionStringBuilder
    {
        string BuildFromSchema(string schema);

        string BuildFromDbName(string dbName);
    }
}