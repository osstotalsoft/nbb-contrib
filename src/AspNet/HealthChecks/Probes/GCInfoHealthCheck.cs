// Copyright (c) TotalSoft.
// This source code is licensed under the MIT license.

using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NBB.Contrib.AspNet.HealthChecks.Probes
{
    public class GCInfoHealthCheck : IHealthCheck
    {
        public string Name { get; } = "GCInfo";

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            // This example will report degraded status if the application is using
            // more than 1gb of memory.
            //
            // Additionally we include some GC info in the reported diagnostics.
            var allocated = GC.GetTotalMemory(forceFullCollection: false);
            var memoryInfo = GC.GetGCMemoryInfo();
            var data = new Dictionary<string, object>()
            {
                { "Allocated", allocated },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) },
                { "Compacted", memoryInfo.Compacted },
                { "Concurrent", memoryInfo.Concurrent },
                { "FinalizationPendingCount", memoryInfo.FinalizationPendingCount },
                { "FragmentedBytes", memoryInfo.FragmentedBytes },
                { "HeapSizeBytes", memoryInfo.HeapSizeBytes },
                { "HighMemoryLoadThresholdBytes", memoryInfo.HighMemoryLoadThresholdBytes },
                { "MemoryLoadBytes", memoryInfo.MemoryLoadBytes },
                { "PauseTimePercentage", memoryInfo.PauseTimePercentage },
                { "PinnedObjectsCount", memoryInfo.PinnedObjectsCount },
                { "PromotedBytes", memoryInfo.PromotedBytes },
                { "TotalAllocatedBytes", GC.GetTotalAllocatedBytes(false) },
                { "TotalAvailableMemoryBytes", memoryInfo.TotalAvailableMemoryBytes },
                { "TotalCommittedBytes", memoryInfo.TotalCommittedBytes },
            };

            // Report degraded status if the allocated memory is >= 1gb (in bytes)
            var status = allocated >= 1024 * 1024 * 1024 ? HealthStatus.Degraded : HealthStatus.Healthy;

            return Task.FromResult(new HealthCheckResult(
                status,
                exception: null,
                description: "reports degraded status if allocated bytes >= 1gb",
                data: data));
        }
    }
}
