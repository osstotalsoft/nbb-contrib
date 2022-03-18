// Copyright (c) TotalSoft.
// This source code is licensed under the MIT license.

using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace NBB.Contrib.AspNet.HealthChecks.Probes.Database
{
    public class SqlConnectionHealthCheck : DbConnectionHealthCheck
    {
        private static readonly string DefaultTestQuery = "Select 1";

        public SqlConnectionHealthCheck(string name, string connectionString)
            : this(name, connectionString, testQuery: DefaultTestQuery)
        {
        }

        public SqlConnectionHealthCheck(string name, string connectionString, string testQuery)
            : base(name, connectionString, testQuery ?? DefaultTestQuery)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
