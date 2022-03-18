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
    public abstract class MultipleDbConnectionHealthCheck : IHealthCheck
    {
        private static readonly string DefaultTestQuery = "Select 1";

        protected MultipleDbConnectionHealthCheck(string name)
            : this(name, testQuery: DefaultTestQuery)
        {
        }

        protected MultipleDbConnectionHealthCheck(string name, string testQuery)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            TestQuery = testQuery;
        }

        public string Name { get; }

        // This sample supports specifying a query to run as a boolean test of whether the database
        // is responding. It is important to choose a query that will return quickly or you risk
        // overloading the database.
        //
        // In most cases this is not necessary, but if you find it necessary, choose a simple query such as 'SELECT 1'.
        protected string TestQuery { get; }

        protected abstract DbConnection CreateConnection(string connectionString);
        protected abstract List<string> GetConnectionStrings();

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object>();
            var hasFailure = false;
            var connectionStrings = GetConnectionStrings();
            for (var i = 0; i < connectionStrings.Count; i++)
            {
                var connectionString = connectionStrings[i];
                var builder = new DbConnectionStringBuilder
                {
                    ConnectionString = connectionString
                };
                if (builder.TryGetValue("Server", out var s))
                {
                    data.Add($"{i} - Server", s);
                }

                if (builder.TryGetValue("Database", out var d))
                {
                    data.Add($"{i} - Database", d);
                }
                using (var connection = CreateConnection(connectionString))
                {
                    try
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();

                        await connection.OpenAsync(cancellationToken);
                        watch.Stop();
                        data.Add($"{i} - ConnectionTimeMs", watch.ElapsedMilliseconds);

                        if (!string.IsNullOrEmpty(TestQuery))
                        {
                            await using var command = connection.CreateCommand();
                            command.CommandText = TestQuery;

                            watch.Restart();
                            await command.ExecuteNonQueryAsync(cancellationToken);
                            watch.Stop();
                            data.Add($"{i} - CommandTimeMs", watch.ElapsedMilliseconds);
                        }
                    }
                    catch (Exception ex)
                    {
                        hasFailure = true;
                        data.Add($"{i} - Exception", ex.Message);
                        data.Add($"{i} - StackTrace", ex.StackTrace);
                    }
                }
            }


            return hasFailure ? HealthCheckResult.Unhealthy("Unhealthy", null, data) : HealthCheckResult.Healthy("Healthy", data);
        }
    }
}
