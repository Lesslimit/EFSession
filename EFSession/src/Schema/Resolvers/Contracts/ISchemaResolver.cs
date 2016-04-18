using EFSession.Schema.Parameters.Contracts;

namespace EFSession.Schema.Resolvers.Contracts
{
    public interface ISchemaResolver
    {
        bool CanResolve(ISchemaCriteria schema);

        string Resolve(ISchemaCriteria criteria);
    }
}