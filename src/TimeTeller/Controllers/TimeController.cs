using Microsoft.AspNetCore.Mvc;
using TimeTeller.Services;

namespace TimeTeller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimeController : ControllerBase
{
    private readonly ILogger logger;
    private readonly RabbitMQService rabbitService;

    public TimeController(ILoggerFactory loggerFactory, RabbitMQService rabbitService)
    {
        logger = loggerFactory.CreateLogger("TimeController");
        this.rabbitService = rabbitService;
    }

    [HttpGet("{timeZoneOffset}")]
    public ActionResult<dynamic> Get(double timeZoneOffset)
    {
        rabbitService.PublishEndpointCalledMessage("GetTime", HttpContext.Connection.RemoteIpAddress!);

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