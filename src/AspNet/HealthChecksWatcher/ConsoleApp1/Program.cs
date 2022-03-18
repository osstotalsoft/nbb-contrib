using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHealthChecksUI()
    .AddInMemoryStorage();

builder.Host.UseSerilog((_, lc) =>
{
    lc
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        ;
});

var app = builder.Build();
app
    .UseRouting()
    .UseEndpoints(config => config.MapHealthChecksUI());

var exitCode = 0;
try
{
    Log.Information("Starting worker");
    Log.Information("Messaging.TopicPrefix=" + builder.Configuration.GetSection("Messaging")["TopicPrefix"]);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    exitCode = 1;
}
finally
{
    Log.CloseAndFlush();
}

Environment.Exit(exitCode);

/*
builder.Services.AddSingleton<CharismaApplicationServer>();
builder.Services.AddScoped<LeasingOfferMapper>();
builder.Services.AddSingleton<IDocumentPartnerMapper, DocumentPartnerMapper>();
builder.Services.AddSingleton<IVerificationEventPublisher, VerificationEventPublisher>();
builder.Services.AddBusinessServices();
builder.Services.AddWriteModelDataAccess();
builder.Services.AddMediatR(typeof(CreateOfferWithAssetCommandHandler).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
builder.Services.SetCacheProvider(builder.Configuration);
builder.Services.AddMessageBus(builder.Configuration);
builder.Services.Decorate<IMessageBusPublisher, LeasingMessageBusPublisherDecorator>();
builder.Services.Decorate<IMessageBusPublisher, OpenTracingPublisherDecorator>();

builder.Services.AddMessagingSubscribers(builder.Configuration);

//TODO: refactor 
//hack to add open generic preprocessor with constraint to be LeasingCommandBase
//https://github.com/aspnet/DependencyInjection/pull/635 
//services.AddMediatRLeasingCommandPreProcessors(new[] {typeof(LeasingCommandPreProcessor<>)},
//  typeof(CreateOfferCommandHandler).Assembly, typeof(CreateOffer).Assembly);

builder.Services.AddLeasingFacades();
builder.Services.AddBLs();
builder.Services.AddApplicationServices();
builder.Services.AddOtherLeasingServices();
builder.Services.AddTsFwkServices();
builder.Services.AddDocumentManagerServices(builder.Configuration.GetValue<bool>("UseExternalDMSService"));
builder.Services.AddBRs();
builder.Services.AddHelpers();
builder.Services.UseVehicleIntegration(builder.Configuration);
builder.Services.UseCASIntegrationService(builder.Configuration.GetSection("CASIntegrationService")["ApiBaseAddress"]);
builder.Services.AddSingleton<ISchemaResolver, JsonSchemaResolver>();
builder.Services.UseTempFilesService(builder.Configuration["DMS:Url"],
    builder.Configuration.GetValue<bool>("DMS:AntivirusCheck"),
    builder.Configuration.GetValue<bool>("DMS:ExtensionCheck"));
builder.Services.AddHttpClient("dms", c => c.BaseAddress = new Uri(builder.Configuration["DMS:Url"]));
builder.Services.AddHttpClient("clamav", c => c.BaseAddress = new Uri(builder.Configuration["CLAMAV:Url"]));
builder.Services.AddAutoMapper(typeof(Charisma.Leasing.WriteApplication.EventHandlers.Orchestrator.FraudCheck.MappingProfile).Assembly);

builder.Services.AddHealthChecks()
    //.AddCheck("Leasing_Database", new SqlConnectionHealthCheck("Leasing_Database", builder.Configuration["ConnectionStrings:Leasing_Database"]))
    .AddCheck<GCInfoHealthCheck>("GC");

// Multitenancy
builder.Services.AddMultitenancy(builder.Configuration)
    .AddMultiTenantMessaging()
    .AddDefaultTenantConfiguration()
    .AddDefaultMessagingTenantIdentification()
    .AddTenantRepository<ConfigurationTenantRepository>();
builder.Configuration.AddJsonFile("appsettings.tenancy.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile("tenancy/appsettings.tenancy.json", optional: true, reloadOnChange: true);

builder.Services.AddJeagerTracing(builder.Configuration);

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    DateTimeZoneHandling = DateTimeZoneHandling.Utc
};


builder.Host.UseSerilog((_, lc) =>
{
    var connectionString = builder.Configuration.GetConnectionString("Log_Database");

    var columnOptions = new ColumnOptions();
    columnOptions.Store.Remove(StandardColumn.Properties);
    columnOptions.Store.Remove(StandardColumn.MessageTemplate);
    columnOptions.Store.Add(StandardColumn.LogEvent);

    lc
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.With<CorrelationLogEventEnricher>()
        .WriteTo.Console()
        .WriteTo.OpenTracing()
        .WriteTo.Conditional(_ => !builder.Environment.IsDevelopment(), wt => wt.MsSqlServerWithCorrelation(connectionString, "__Logs", autoCreateSqlTable: true, columnOptions: columnOptions));
});

var app = builder.Build();

app.Services.UseUnityContainerDelegation();
var srv = app.Services.GetRequiredService<CharismaApplicationServer>();
srv.Initialize();

app.UseHealthChecks("/health", options: new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


var exitCode = 0;
try
{
    Log.Information("Starting worker");
    Log.Information("Messaging.TopicPrefix=" + builder.Configuration.GetSection("Messaging")["TopicPrefix"]);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    exitCode = 1;
}
finally
{
    Log.CloseAndFlush();
}
*/
//Environment.Exit(exitCode);
