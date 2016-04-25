using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace EFSession
{
    public class SqlServerConnectionStringBuilder : ISqlServerConnectionStringBuilder
    {
        private readonly string originalConnectionString;
        private readonly string defaultSchema;

//        public SqlServerConnectionStringBuilder(IConfig config)
//            : this(config.GetStringOrThrow("DatabaseConnectionString"),
//                  config.GetStringOrThrow("DefaultDbSchema"))
//        {
//        }

        public SqlServerConnectionStringBuilder(string connectionString, string defaultSchema)
        {
            originalConnectionString = connectionString;
            this.defaultSchema = defaultSchema;
        }

        public string BuildFromSchema(string schema)
        {
            // ReSharper disable once CollectionNeverQueried.Local
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(originalConnectionString)
                {
                    AsynchronousProcessing = true
                };

            if (string.Compare(schema, defaultSchema, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return sqlConnectionStringBuilder.ConnectionString;
            }

            var regex = new Regex("@(.*)$");
            var atServerName = regex.Match(sqlConnectionStringBuilder.UserID);

            if (atServerName.Success)
            {
                var rootUserName = regex.Replace(sqlConnectionStringBuilder.UserID, string.Empty);
                sqlConnectionStringBuilder.UserID = $"{rootUserName}.{schema}{atServerName}";
            }

            return sqlConnectionStringBuilder.ConnectionString;
        }

        public string BuildFromDbName(string dbName)
        {
            // ReSharper disable once CollectionNeverQueried.Local
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(originalConnectionString)
            {
                AsynchronousProcessing = true,
                InitialCatalog = dbName
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }
    }
}