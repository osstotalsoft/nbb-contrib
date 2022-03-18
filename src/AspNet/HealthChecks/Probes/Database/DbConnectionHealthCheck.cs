// Copyright (c) TotalSoft.
// This source code is licensed under the MIT license.

using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.Contrib.AspNet.HealthChecks.Probes.Database
{
    public abstract class DbConnectionHealthCheck : IHealthCheck
    {
        protected DbConnectionHealthCheck(string name, string connectionString)
            : this(name, connectionString, testQuery: null)
        {
        }

        protected DbConnectionHealthCheck(string name, string connectionString, string testQuery)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            TestQuery = testQuery;
        }

        public string Name { get; }

        protected string ConnectionString { get; }

        // This sample supports specifying a query to run as a boolean test of whether the database
        // is responding. It is important to choose a query that will return quickly or you risk
        // overloading the database.
        //
        // In most cases this is not necessary, but if you find it necessary, choose a simple query such as 'SELECT 1'.
        protected string TestQuery { get; }

        protected abstract DbConnection CreateConnection(string connectionString);

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var data = new Dictionary<string, object>();
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = ConnectionString
            };
            if (builder.TryGetValue("Server", out var s))
            {
                data.Add("Server", s);
            }

            if (builder.TryGetValue("Database", out var d))
            {
                data.Add("Database", d);
            }
            using (var connection = CreateConnection(ConnectionString))
            {
                try
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    await connection.OpenAsync(cancellationToken);
                    watch.Stop();
                    data.Add("ConnectionTimeMs", watch.ElapsedMilliseconds);

                    if (TestQuery != null)
                    {
                        await using var command = connection.CreateCommand();
                        command.CommandText = TestQuery;

                        watch.Restart();
                        await command.ExecuteNonQueryAsync(cancellationToken);
                        watch.Stop();
                        data.Add("CommandTimeMs", watch.ElapsedMilliseconds);

                    }
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy(ex.Message, ex, data);
                }
            }

            return HealthCheckResult.Healthy("Healthy", data);
        }
    }
}
