using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EFSession.Extensions
{
    public static class SqlParameterEnumerableExt
    {
        public static string ToSpParamsString(this IEnumerable<SqlParameter> sqlParameters)
        {
            return string.Join(",", sqlParameters.Select(sqlp => sqlp.Direction == ParameterDirection.Output
                ? $"{sqlp.ParameterName} OUTPUT"
                : sqlp.ParameterName));
        }
    }
}