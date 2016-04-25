using System;
using System.Linq.Expressions;

namespace EFSession.Extensions
{
    public static class ExpressionExtensions
    {
        public static bool TryGetName<TSource, TField>(this Expression<Func<TSource, TField>> expression, out string name)
        {
            try
            {
                name = (expression.Body as MemberExpression ?? (MemberExpression) ((UnaryExpression) expression.Body).Operand).Member.Name;

                return !string.IsNullOrEmpty(name);
            }
            catch
            {
                name = null;

                return false;
            }
        }
    }
}