using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace EFSession.Extensions
{
    public static class DbContextExtension
    {
        public static ObjectContext GetObjectContext(this DbContext dbContext)
        {
            return ((IObjectContextAdapter)dbContext).ObjectContext;
        }
    }
}