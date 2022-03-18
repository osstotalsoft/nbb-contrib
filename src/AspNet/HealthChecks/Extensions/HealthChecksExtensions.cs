// Copyright (c) TotalSoft.
// This source code is licensed under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NBB.Contrib.AspNet.HealthChecks.Extensions
{
    public static class HealthChecksExtensions
    {
        public static IApplicationBuilder UseDefaultHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks($"/{HealthCheckEndpoints.Liveness}", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                ResponseWriter = HealthCheckWriter.WriteResponse,
                Predicate = healthCheck => healthCheck.Tags.Contains(HealthCheckEndpoints.Liveness)
            });

            app.UseHealthChecks($"/{HealthCheckEndpoints.Readyness}", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                ResponseWriter = HealthCheckWriter.WriteResponse,
                Predicate = healthCheck => healthCheck.Tags.Contains(HealthCheckEndpoints.Readyness)
            });

            return app;
        }
    }
}
