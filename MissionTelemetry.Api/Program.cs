using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MissionTelemetry.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer(); // für Swagger
builder.Services.AddSwaggerGen();           // Swagger-Gen
builder.Services.AddSingleton<IAlarmEvaluator>(sp =>
{
    var path = Path.Combine(AppContext.BaseDirectory, "mission_dict.json");
    var loader = new JsonDictionaryLoader();
    var dict = loader.LoadFromFile(path);
    return new DataDrivenAlarmEvaluator(dict);
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // <-- UI aktiv
}

app.UseHttpsRedirection();
app.MapControllers();


app.Run();


