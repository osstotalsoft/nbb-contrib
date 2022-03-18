// Copyright (c) TotalSoft.
// This source code is licensed under the MIT license.

using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NBB.Contrib.AspNet.HealthChecks
{
    public class HealthCheckWriter
    {
        public static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            // only use this if json formatting is requested. This is needed for HealthCheckUi package to work
            if (httpContext.Request.QueryString.HasValue)
            {
                var queryString = httpContext.Request.QueryString.Value;
                queryString = queryString.Replace("?", "");
                if (!"json".Equals(queryString, StringComparison.InvariantCultureIgnoreCase))
                {
                    return UIResponseWriter.WriteHealthCheckUIResponse(httpContext, result);
                }
            }
            else
            {
                return UIResponseWriter.WriteHealthCheckUIResponse(httpContext, result);
            }
            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("exception", pair.Value.Exception != null ? pair.Value.Exception.Message : ""),
                        new JProperty("stackTrace", pair.Value.Exception != null ? pair.Value.Exception.StackTrace : ""),
                        new JProperty("data", new JObject(pair.Value.Data.Select(p => new JProperty(p.Key, p.Value))))))))));
            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    }
}
