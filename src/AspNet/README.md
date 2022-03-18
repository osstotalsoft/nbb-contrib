# Asp.Net packages
We offer helpers for asp.net core 6 projects.

Packages
----------------
* NBB.Contrib.AspNet.HealthChecks - Health checks can be used to expose to outside systems the state of the service / application.

Usage - `Startup.cs`:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    var healthBuilder = services.AddHealthChecks();
    healthBuilder.AddCheck<GCInfoHealthCheck>("GC", null, HealthCheckEndpoints.All);
    healthBuilder.AddTypeActivatedCheck<MultiTenantSqlConnectionHealthCheck>("My_Database", null, HealthCheckEndpoints.All, "My_Database");
    healthBuilder.AddTypeActivatedCheck<IdentityHealthCheck>("IdentityServer", null, HealthCheckEndpoints.All, Configuration.GetValue("Identity:Authority", ""));
}
```
One special probe is the MultiTenantSqlConnectionHealthCheck. This probe will use the NBB (https://github.com/osstotalsoft/nbb) multi tenancy infrastructure to get the connection strings for all tenants and a specific config key / service. In the above example, we will get all connection strings for the connection string key "My_Database".

Configuration of the web application in `Startup.cs`. 

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseDefaultHealthChecks();
}
```
This code will add 2 health endpoints:
* /livez: used to check that the service is live
* /readyz: used to check is the service is started 

Probes can be added to one endpoint or to both. 

Constants:
- HealthCheckEndpoints.All refers to /livez and readyz. 
- HealthCheckEndpoints.Liveness is /livez. 
- HealthCheckEndpoints.Readyness is /readyz

Those endpoints are not automatically called, they need to be called by a 3rd party system.

Kubernetes can be configured to automatically call those endpoints. 
https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-startup-probes/
https://kubernetes.io/docs/reference/using-api/health-checks/
