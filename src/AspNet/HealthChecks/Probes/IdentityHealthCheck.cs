// Copyright (c) TotalSoft.
// This source code is licensed under the MIT license.

using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.Contrib.AspNet.HealthChecks.Probes
{
    public class IdentityHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public IdentityHealthCheck(HttpClient httpClient, string url)
        {
            _httpClient = httpClient;
            _url = url;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object>
            {
                { "IdentityUrl", _url }
            };

            if (string.IsNullOrEmpty(_url))
            {
                return HealthCheckResult.Unhealthy($"Invalid identity url {_url}", null, data);
            }
            var uri = new Uri(new Uri(_url), "/.well-known/openid-configuration");
            try
            {
                var response = await _httpClient.GetAsync(uri, cancellationToken);
                data.Add("StatusCode", response.StatusCode);
                data.Add("IsSuccessStatusCode", response.IsSuccessStatusCode);

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                data.Add("Response", content);
                return response.IsSuccessStatusCode ? HealthCheckResult.Healthy("OK", data) : HealthCheckResult.Unhealthy(content, null, data);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message, ex, data);
            }
        }
    }
}
