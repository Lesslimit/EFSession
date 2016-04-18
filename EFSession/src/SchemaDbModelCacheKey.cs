using System.Data.Entity.Infrastructure;

namespace EFSession
{
    public class SchemaDbModelCacheKey : IDbModelCacheKey
    {
        private readonly string schemaName;

        public SchemaDbModelCacheKey(string schemaName)
        {
            this.schemaName = schemaName;
        }

        protected bool Equals(SchemaDbModelCacheKey other)
        {
            return string.Equals(schemaName, other.schemaName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((SchemaDbModelCacheKey) obj);
        }

        public override int GetHashCode()
        {
            return schemaName?.GetHashCode() ?? 0;
        }
    }
}