using System.Reflection;

namespace EFSession.Emit.Contracts
{
    public delegate object ObjectConstructor<T>(params object[] args);

    public interface IDymanicObjectCreator
    {
        ObjectConstructor<object> CreateParametrizedConstructor(MethodBase method);
    }
}