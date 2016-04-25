using System;
using System.Reflection;
using EFSession.Emit.Contracts;

namespace EFSession.Emit
{
    public delegate TResult MethodCall<in T, out TResult>(T target, params object[] args);

    public abstract class ReflectionDelegateFactory : IDymanicObjectCreator
    {
        public abstract MethodCall<T, object> CreateMethodCall<T>(MethodBase method);
        public abstract ObjectConstructor<object> CreateParametrizedConstructor(MethodBase method);
        public abstract Func<T> CreateDefaultConstructor<T>(Type type);
        public abstract Func<T, object> CreateGet<T>(PropertyInfo propertyInfo);
        public abstract Func<T, object> CreateGet<T>(FieldInfo fieldInfo);
        public abstract Action<T, object> CreateSet<T>(FieldInfo fieldInfo);
        public abstract Action<T, object> CreateSet<T>(PropertyInfo propertyInfo);

        public Func<T, object> CreateGet<T>(MemberInfo memberInfo)
        {
            PropertyInfo propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return CreateGet<T>(propertyInfo);
            }

            FieldInfo fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return CreateGet<T>(fieldInfo);
            }

            throw new Exception($"Could not create getter for {memberInfo}.");
        }

        public Action<T, object> CreateSet<T>(MemberInfo memberInfo)
        {
            PropertyInfo propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return CreateSet<T>(propertyInfo);
            }

            FieldInfo fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return CreateSet<T>(fieldInfo);
            }

            throw new Exception($"Could not create setter for {memberInfo}.");
        }
    }
}