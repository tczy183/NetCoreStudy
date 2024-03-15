using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region openTelemetry
//
// // Custom metrics for the application
// var greeterMeter = new Meter("OtPrGrYa.Example", "1.0.0");
// var countGreetings = greeterMeter.CreateCounter<int>("greetings.count", description: "Counts the number of greetings");
//
// // Custom ActivitySource for the application
// var greeterActivitySource = new ActivitySource("OtPrGrJa.Example");
//
//
// var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
// var otel = builder.Services.AddOpenTelemetry();
//
// // Configure OpenTelemetry Resources with the application name
// otel.ConfigureResource(resource => resource
//     .AddService(serviceName: builder.Environment.ApplicationName));
//
// // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
// otel.WithMetrics(metrics => metrics
//     // Metrics provider from OpenTelemetry
//     .AddAspNetCoreInstrumentation()
//     .AddMeter(greeterMeter.Name)
//     // Metrics provides by ASP.NET Core in .NET 8
//     .AddMeter("Microsoft.AspNetCore.Hosting")
//     .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
//     .AddPrometheusExporter());
//
// // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
// otel.WithTracing(tracing =>
// {
//     tracing.AddAspNetCoreInstrumentation();
//     tracing.AddHttpClientInstrumentation();
//     tracing.AddSource(greeterActivitySource.Name);
//     if (tracingOtlpEndpoint != null)
//     {
//         tracing.AddOtlpExporter(otlpOptions => { otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint); });
//     }
//     else
//     {
//         tracing.AddConsoleExporter();
//     }
// });

#endregion

#region mediatr

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.AddBehavior<LoggerBehavior>();
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/weatherforecastmediatr", async ([FromServices] ISender sender) =>
    {
        var send = await sender.Send(new WeatherForecastRequest("Test"));
        return send;
    })
    .WithName("GetWeatherForecastMediatR")
    .WithOpenApi();

#region OpenTelemetry

// app.MapGet("/", SendGreeting);
//
// async Task<String> SendGreeting(ILogger<Program> logger)
// {
//     // Create a new Activity scoped to the method
//     using var activity = greeterActivitySource.StartActivity("GreeterActivity");
//
//     // Log a message
//     logger.LogInformation("Sending greeting");
//
//     // Increment the custom counter
//     countGreetings.Add(1);
//
//     // Add a tag to the Activity
//     activity?.SetTag("greeting", "Hello World!");
//
//     return "Hello World!";
// }
//
// app.MapPrometheusScrapingEndpoint();

#endregion

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

#region MediatR
public class WeatherForecastRequest(string Name) : IRequest<WeatherForecast[]>
{
}

public class WeatherForecastMediatRHandler : IRequestHandler<WeatherForecastRequest, WeatherForecast[]>
{
    public Task<WeatherForecast[]> Handle(WeatherForecastRequest request, CancellationToken cancellationToken)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return Task.FromResult(forecast);
    }
}

public class LoggerBehavior(ILogger<LoggerBehavior> logger):IPipelineBehavior<WeatherForecastRequest,WeatherForecast[]>
{
    public async Task<WeatherForecast[]> Handle(WeatherForecastRequest request, RequestHandlerDelegate<WeatherForecast[]> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("-- Handling Request");
        var response = await next();
        logger.LogInformation("-- Finished Request");
        return response;
    }
}

#endregion
