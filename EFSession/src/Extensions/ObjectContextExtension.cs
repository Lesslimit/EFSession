using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace EFSession.Extensions
{
    public static class ObjectContextExtension
    {
        public static bool IsAttached(this ObjectContext objectContext, object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            ObjectStateEntry entry;
            if (objectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
            {
                return (entry.State != EntityState.Detached);
            }

            return false;
        }

        public static void DetachSafely(this ObjectContext objectContext, object entity)
        {
            if (objectContext.IsAttached(entity))
            {
                objectContext.Detach(entity);
            }
        }
    }
}