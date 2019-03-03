using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace TimeTeller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeController : ControllerBase
    {
        private readonly ILogger logger;

        public TimeController(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("TimeController");
        }

        [HttpGet("{timeZoneOffset}")]
        public ActionResult<dynamic> Get(double timeZoneOffset)
        {
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
}