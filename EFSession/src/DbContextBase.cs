using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace EFSession
{
    public abstract class DbContextBase : DbContext, IDbContext
    {
        public virtual bool ConnectionIsPreserved { get; }

        public virtual void MarkConnectionAsPreserved()
        {
        }

        public virtual void ReleasePreservedConnection()
        {
        }

        public ObjectContext GetObjectContext()
        {
            return ((IObjectContextAdapter)this).ObjectContext;
        }
    }
}