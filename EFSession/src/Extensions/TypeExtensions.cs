using System;
using System.Collections.Generic;
using EFSession.Enums;

namespace EFSession.Extensions
{
    public static class TypeExtensions
    {
        private static readonly Dictionary<Type, PrimitiveTypeCode> typeCodeMap =
            new Dictionary<Type, PrimitiveTypeCode>
            {
                {typeof(char), PrimitiveTypeCode.Char},
                {typeof(char?), PrimitiveTypeCode.CharNullable},
                {typeof(bool), PrimitiveTypeCode.Boolean},
                {typeof(bool?), PrimitiveTypeCode.BooleanNullable},
                {typeof(sbyte), PrimitiveTypeCode.SByte},
                {typeof(sbyte?), PrimitiveTypeCode.SByteNullable},
                {typeof(short), PrimitiveTypeCode.Int16},
                {typeof(short?), PrimitiveTypeCode.Int16Nullable},
                {typeof(ushort), PrimitiveTypeCode.UInt16},
                {typeof(ushort?), PrimitiveTypeCode.UInt16Nullable},
                {typeof(int), PrimitiveTypeCode.Int32},
                {typeof(int?), PrimitiveTypeCode.Int32Nullable},
                {typeof(byte), PrimitiveTypeCode.Byte},
                {typeof(byte?), PrimitiveTypeCode.ByteNullable},
                {typeof(uint), PrimitiveTypeCode.UInt32},
                {typeof(uint?), PrimitiveTypeCode.UInt32Nullable},
                {typeof(long), PrimitiveTypeCode.Int64},
                {typeof(long?), PrimitiveTypeCode.Int64Nullable},
                {typeof(ulong), PrimitiveTypeCode.UInt64},
                {typeof(ulong?), PrimitiveTypeCode.UInt64Nullable},
                {typeof(float), PrimitiveTypeCode.Single},
                {typeof(float?), PrimitiveTypeCode.SingleNullable},
                {typeof(double), PrimitiveTypeCode.Double},
                {typeof(double?), PrimitiveTypeCode.DoubleNullable},
                {typeof(DateTime), PrimitiveTypeCode.DateTime},
                {typeof(DateTime?), PrimitiveTypeCode.DateTimeNullable},
                {typeof(DateTimeOffset), PrimitiveTypeCode.DateTimeOffset},
                {typeof(DateTimeOffset?), PrimitiveTypeCode.DateTimeOffsetNullable},
                {typeof(decimal), PrimitiveTypeCode.Decimal},
                {typeof(decimal?), PrimitiveTypeCode.DecimalNullable},
                {typeof(Guid), PrimitiveTypeCode.Guid},
                {typeof(Guid?), PrimitiveTypeCode.GuidNullable},
                {typeof(TimeSpan), PrimitiveTypeCode.TimeSpan},
                {typeof(TimeSpan?), PrimitiveTypeCode.TimeSpanNullable},
                {typeof(Uri), PrimitiveTypeCode.Uri},
                {typeof(string), PrimitiveTypeCode.String},
                {typeof(byte[]), PrimitiveTypeCode.Bytes},
                {typeof(DBNull), PrimitiveTypeCode.DBNull}
            };

        public static object GetDefaultValuePerf(this Type type)
        {
            if (!type.IsValueType)
            {
                return null;
            }

            switch (GetTypeCode(type))
            {
                case PrimitiveTypeCode.Boolean:
                    return false;
                case PrimitiveTypeCode.Char:
                case PrimitiveTypeCode.SByte:
                case PrimitiveTypeCode.Byte:
                case PrimitiveTypeCode.Int16:
                case PrimitiveTypeCode.UInt16:
                case PrimitiveTypeCode.Int32:
                case PrimitiveTypeCode.UInt32:
                    return 0;
                case PrimitiveTypeCode.Int64:
                case PrimitiveTypeCode.UInt64:
                    return 0L;
                case PrimitiveTypeCode.Single:
                    return 0f;
                case PrimitiveTypeCode.Double:
                    return 0.0;
                case PrimitiveTypeCode.Decimal:
                    return 0m;
                case PrimitiveTypeCode.DateTime:
                    return new DateTime();
                case PrimitiveTypeCode.Guid:
                    return new Guid();
                case PrimitiveTypeCode.DateTimeOffset:
                    return new DateTimeOffset();
            }

            return IsNullable(type) ? null : Activator.CreateInstance(type);
        }

        public static PrimitiveTypeCode GetTypeCode(Type t)
        {
            PrimitiveTypeCode typeCode;
            if (typeCodeMap.TryGetValue(t, out typeCode))
            {
                return typeCode;
            }

            if (t.IsEnum)
            {
                return GetTypeCode(Enum.GetUnderlyingType(t));
            }

            // performance?
            if (t.IsNullable())
            {
                Type nonNullable = Nullable.GetUnderlyingType(t);
                if (nonNullable.IsEnum)
                {
                    Type nullableUnderlyingType = typeof(Nullable<>).MakeGenericType(Enum.GetUnderlyingType(nonNullable));

                    return GetTypeCode(nullableUnderlyingType);
                }
            }

            return PrimitiveTypeCode.Object;
        }

        public static bool IsNullable(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}