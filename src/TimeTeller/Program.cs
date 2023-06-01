using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Serilog;
using System.Reflection;
using TimeTeller.Services;

// Setting Serilog up.
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true, reloadOnChange: true)
    .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("-------------------------------------");
    Log.Information($"TimeTeller version {Assembly.GetExecutingAssembly().GetName().Version} started :)");
    Log.Information("-------------------------------------");

    var builder = WebApplication.CreateBuilder(args);

    // Service registrations.
    builder.Host.UseSerilog();
    builder.Host.UseMetrics(options =>
    {
        options.EndpointOptions = epo =>
        {
            epo.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
            epo.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
        };
    });
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<RabbitMQService>();

    var app = builder.Build();

    // Building the pipeline.
    app.UseSerilogRequestLogging();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.MapControllers();
    app.MapFallback(r => r.Response.WriteAsync("(: TimeTeller is running :)"));
    app.Run();
}
catch (Exception ex)
{
    Log.Information("-----------------------------------");
    Log.Fatal(ex, "TimeTeller terminated unexpectedly :(");
    Log.Information("-----------------------------------");
}
finally
{
    Log.CloseAndFlush();
}