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
    public class HttpHealthCheck : IHealthCheck
    {
        private readonly HttpHealthCheckOptions _options;

        public HttpHealthCheck(HttpHealthCheckOptions options)
        {
            _options = options;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var url = _options.Url;
            var data = new Dictionary<string, object>
            {
                { "IdentityUrl", url }
            };

            if (string.IsNullOrEmpty(url))
            {
                return HealthCheckResult.Unhealthy($"Invalid url {url}", null, data);
            }

            var httpClient = new HttpClient();
            foreach (var (key, value) in _options.Headers)
            {
                httpClient.DefaultRequestHeaders.Add(key, value);
            }

            var uri = new Uri(url);
            try
            {
                HttpResponseMessage response;
               
                HttpMethod method = _options.Method == "POST" ? HttpMethod.Post : HttpMethod.Get;
                var request = new HttpRequestMessage(method, uri);
                if (!string.IsNullOrEmpty(_options.Payload))
                {
                    request.Content = new StringContent(_options.Payload);
                }
               
                response = await httpClient.SendAsync(request, cancellationToken);
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

    public class HttpHealthCheckOptions
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Headers = new();
        public string Payload { get; set; }
    }
}
