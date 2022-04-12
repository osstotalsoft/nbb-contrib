// Copyright (c) TotalSoft.
// This source code is licensed under the MIT license.

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.Contrib.AspNet.HealthChecks.Probes
{
    public class SmtpHealthCheck : IHealthCheck
    {
        private readonly SmtpCheckOptions _smtpCheckOptions;
        private readonly ILogger<SmtpHealthCheck> _logger;

        public SmtpHealthCheck(SmtpCheckOptions smtpCheckOptions, ILogger<SmtpHealthCheck> logger)
        {
            _smtpCheckOptions = smtpCheckOptions;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var data = new Dictionary<string, object>()
                      {
                        { "server", _smtpCheckOptions.Server },
                        { "port", _smtpCheckOptions.Port },
                        { "useSSL", _smtpCheckOptions.UseSsl },
                        { "useAuth", _smtpCheckOptions.UseAuthentication },
                      };

            try
            {
                var response = "";

                using (var client = new TcpClient())
                {
                    var server = _smtpCheckOptions.Server;
                    var port = _smtpCheckOptions.Port;
                    client.Connect(server, port);
                    if (!client.Connected)
                    {
                        return HealthCheckResult.Unhealthy($"Email server {_smtpCheckOptions.Server}:{_smtpCheckOptions.Port} not reacheable because the client socket did not connect", data: data);
                    }
                    using var stream = client.GetStream();
                    if (_smtpCheckOptions.UseSsl)
                    {
                        using var sslStream = new SslStream(stream);
                        sslStream.AuthenticateAsClient(server);
                        using var writer = new StreamWriter(sslStream);
                        using var reader = new StreamReader(sslStream);
                        await writer.WriteLineAsync("EHLO " + server);
                        await writer.FlushAsync();
                        response = reader.ReadLine();
                        _logger.LogDebug("Smtp Server response {Response}", response);
                    }
                    else
                    {
                        using var writer = new StreamWriter(stream);
                        using var reader = new StreamReader(stream);
                        await writer.WriteLineAsync("EHLO " + server);
                        await writer.FlushAsync();
                        response = reader.ReadLine();
                        _logger.LogDebug("Smtp Server response {Response}", response);
                    }
                }
                return HealthCheckResult.Healthy($"Email server {_smtpCheckOptions.Server}:{_smtpCheckOptions.Port} response {response} connection OK", data: data);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Email server {_smtpCheckOptions.Server}:{_smtpCheckOptions.Port} not reachable {ex.Message} - {ex.StackTrace}";
                _logger.LogError(ex, "Error checking mail server {errorMessage}", errorMessage);
                return HealthCheckResult.Unhealthy(errorMessage, ex, data);
            }
        }

        public class SmtpCheckOptions
        {
            public string Server { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Domain { get; set; }
            public string Password { get; set; }
            public bool UseAuthentication { get; set; }
            public bool UseSsl { get; set; }
        }
    }
}
