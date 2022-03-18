// Copyright (c) TotalSoft.
// This source code is licensed under the MIT license.

using Microsoft.Data.SqlClient;
using NBB.MultiTenancy.Abstractions.Configuration;
using System.Collections.Generic;
using System.Data.Common;

namespace NBB.Contrib.AspNet.HealthChecks.Probes.Database
{
    public class MultiTenantSqlConnectionHealthCheck : MultipleDbConnectionHealthCheck
    {
        private readonly TenantInfrastructure _tenantInfrastructure;
        public MultiTenantSqlConnectionHealthCheck(TenantInfrastructure tenantInfrastructure, string name):  base(name)
        {
            _tenantInfrastructure = tenantInfrastructure;
        }

        protected override List<string> GetConnectionStrings()
        {
            var connectionStrings = _tenantInfrastructure.GetConnectionStrings(this.Name).GetAwaiter().GetResult();
            return connectionStrings;
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
