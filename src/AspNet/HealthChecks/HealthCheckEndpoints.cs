// Copyright (c) TotalSoft.
// This source code is licensed under the MIT license.

using System.Collections.Generic;

namespace NBB.Contrib.AspNet.HealthChecks
{
    public static class HealthCheckEndpoints
    {
        public const string Liveness = "livez";
        public const string Readyness = "readyz";
        public static IReadOnlyList<string> All = new List<string> { Liveness, Readyness };
    }
}
