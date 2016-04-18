using EFSession.Schema.Parameters.Contracts;

namespace EFSession.Schema.Parameters
{
    public struct SchemaNameCriteria : ISchemaCriteria
    {
        public string SchemaName { get; private set; }

        public SchemaNameCriteria(string schemaName)
        {
            SchemaName = schemaName;
        }

        public static implicit operator SchemaNameCriteria(string schemaName)
        {
            return new SchemaNameCriteria(schemaName);
        }
    }
}