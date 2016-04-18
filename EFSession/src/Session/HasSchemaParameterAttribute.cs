using System;

namespace EFSession.Session
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class HasSchemaParameterAttribute : Attribute
    {
        public int Order { get; }

        public string Name { get; }

        public HasSchemaParameterAttribute(int order = 1, string name = "AgencySchemaName")
        {
            Order = order;
            Name = name;
        }
    }
}