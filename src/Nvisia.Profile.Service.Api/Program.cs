using Nvisia.Profile.Service.Api;
using Nvisia.Profile.Service.Domain;
using Nvisia.Profile.Service.WriteStore;
using Serilog;

// Initialize WebApplicationBuilder
var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsProduction())
{
    builder.Configuration
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables();
}
else
{
    builder.Configuration
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json");
}

// Configure logging
builder.ConfigureSerilog();


// Setup dependency injection
builder.Services
    .AddApiControllers()
    .AddDefaultCors()
    .AddApiHealthChecks()
    .AddApiExceptionHandlers()
    .AddApiMappers()
    .AddApiValidation()
    .AddWriteStoreContext(builder.Configuration)
    .AddDomainServices()
    .AddDomainMappers()
    .AddSwagger();


// Configure builder
var app = builder.Build();
app.ConfigureApi()
    .UseSerilogRequestLogging()
    .ConfigureSwagger();


// Start the application
Log.Information("Starting Profile Service Application");
app.Run();