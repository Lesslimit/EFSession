using EFSession.Schema.Parameters;
using EFSession.Schema.Parameters.Contracts;
using EFSession.Schema.Resolvers.Contracts;

namespace EFSession.Schema.Resolvers
{
    public class SchemaNameSchemaResolver : ISchemaResolver
    {
        public bool CanResolve(ISchemaCriteria schema)
        {
            return schema is SchemaNameCriteria;
        }

        public string Resolve(ISchemaCriteria criteria)
        {
            return ((SchemaNameCriteria) criteria).SchemaName;
        }
    }
}