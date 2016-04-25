using EFSession.Schema.Parameters;
using EFSession.Schema.Parameters.Contracts;
using EFSession.Schema.Resolvers.Contracts;

namespace EFSession.Schema.Resolvers
{
    public class ConstantSchemaResolver : ISchemaResolver
    {
        public bool CanResolve(ISchemaCriteria schema)
        {
            return schema is DefaultCriteria;
        }

        public string Resolve(ISchemaCriteria criteria)
        {
            return ((DefaultCriteria) criteria).Value;
        }
    }
}