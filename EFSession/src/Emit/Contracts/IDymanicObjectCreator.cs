using System.Reflection;

namespace EFSession.Emit.Contracts
{
    public interface IDymanicObjectCreator
    {
        ObjectConstructor<object> CreateParametrizedConstructor(MethodBase method);
    }
}