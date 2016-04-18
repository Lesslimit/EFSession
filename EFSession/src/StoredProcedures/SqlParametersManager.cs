using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EFSession.StoredProcedures
{
    public class SqlParametersManager : ISqlParametersManager
    {
        private const string At = "@";

        public SqlParameter[] PrepareParameters(object parameters, params SqlParameter[] outputParameters)
        {
            var parameterParameters = new List<SqlParameter>();

            if (parameters != null)
            {
                parameterParameters.AddRange(parameters.GetType()
                    .GetProperties()
                    .Select(propertyInfo => new { propertyInfo, name = At + propertyInfo.Name })
                    .Select(_ => new { _, value = _.propertyInfo.GetValue(parameters, null) })
                    .Select(_ => new SqlParameter(_._.name, _.value ?? DBNull.Value)));
            }

            foreach (var outputParameter in outputParameters)
            {
                if (outputParameter.Direction != ParameterDirection.Output)
                {
                    outputParameter.Direction = ParameterDirection.Output;
                }

                if (!outputParameter.ParameterName.StartsWith(At))
                {
                    outputParameter.ParameterName = At + outputParameter.ParameterName;
                }
            }

            parameterParameters.AddRange(outputParameters);

            return parameterParameters.ToArray();
        }
    }

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