using App.Metrics;
using App.Metrics.Meter;
using Microsoft.AspNetCore.Mvc;
using TimeTeller.Services;

namespace TimeTeller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimeController : ControllerBase
{
    private readonly ILogger logger;
    private readonly RabbitMQService rabbitService;
    private readonly IMetrics metrics;
    private static readonly MeterOptions hitsMeter = new() { Name = "Time Hits", MeasurementUnit = Unit.Percent, RateUnit = TimeUnit.Seconds };

    public TimeController(ILoggerFactory loggerFactory, RabbitMQService rabbitService, IMetrics metrics)
    {
        logger = loggerFactory.CreateLogger("TimeController");
        this.rabbitService = rabbitService;
        this.metrics = metrics;
    }

    [HttpGet("{timeZoneOffset}")]
    public ActionResult<dynamic> Get(double timeZoneOffset)
    {
        rabbitService.PublishEndpointCalledMessage("GetTime", HttpContext.Connection.RemoteIpAddress!);
        metrics.Measure.Meter.Mark(
            hitsMeter,
            timeZoneOffset switch
            {
                3.5 => new MetricSetItem("Tehran", "3.5"),
                4 => new MetricSetItem("Yerevan", "4"),
                _ => new MetricSetItem()
            }
        );

        dynamic result = new
        {
            Time = DateTime.UtcNow.AddHours(timeZoneOffset).ToLongTimeString(),
            TimeZoneOffset = timeZoneOffset
        };

        string log = result.ToString();
        logger.LogInformation(log);

        return result;
    }
}