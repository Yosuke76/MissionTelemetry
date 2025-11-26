using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MissionTelemetry.Api.Repositories;
using MissionTelemetry.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();     // Microsoft.AspNetCore.OpenApi
builder.Services.AddControllers();

builder.Services.AddSingleton<ITelemetryRepository, InMemoryTelemetryRepository>();
builder.Services.AddSingleton<IProximityRepository, InMemoryProximityRepository>();
builder.Services.AddSingleton<IAlarmReadModel, AlarmReadModelAdapter>();

builder.Services.AddSingleton<IAlarmEvaluator>(sp =>
{
    var path = Path.Combine(AppContext.BaseDirectory, "mission_dict.json");
    var dict = new JsonDictionaryLoader().LoadFromFile(path);
    return new DataDrivenAlarmEvaluator(dict);
});

builder.Services.AddSingleton<IAlarmManager, AlarmManager>();

builder.Services.AddSingleton<IProximitySource>(_ => new SimulatedProximitySource(1.0));
builder.Services.AddSingleton<ITelemtrySource>(_ => new SimulatedTelemetrySource(1.0));
builder.Services.AddHostedService<MissionTelemetry.Api.Services.SimulationWorker>();

var app = builder.Build();

// OpenAPI JSON IMMER verfügbar (nicht nur in Development)
app.MapOpenApi(); // -> /openapi/v1.json

// ReDoc UI unter /docs (alternative zu Swagger UI)
app.MapGet("/docs", () =>
{
    var html = """
    <!doctype html>
    <html>
    <head>
      <meta charset="utf-8"/>
      <title>MissionTelemetry API Docs</title>
      <meta name="viewport" content="width=device-width, initial-scale=1">
      <style>html,body{height:100%;margin:0}</style>
    </head>
    <body>
      <redoc spec-url="/openapi/v1.json"></redoc>
      <script src="https://cdn.redoc.ly/redoc/latest/bundles/redoc.standalone.js"></script>
    </body>
    </html>
    """;
    return Results.Content(html, "text/html");
});

// Kleine Startseite für "/"
app.MapGet("/", () =>
    Results.Text("MissionTelemetry API läuft.\nDocs: /docs\nOpenAPI: /openapi/v1.json",
                 "text/plain"));

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
