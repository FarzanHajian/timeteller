using LiteDB;
using Microsoft.AspNetCore.Mvc;
using TimeTeller.Services;

namespace TimeTeller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly RabbitMQService rabbitService;

    public AdminController(IConfiguration configuration, RabbitMQService rabbitService)
    {
        this.configuration = configuration;
        this.rabbitService = rabbitService;
    }

    [HttpGet("logs")]
    public string GetLogs()
    {
        rabbitService.PublishEndpointCalledMessage("GetLogs", HttpContext.Connection.RemoteIpAddress!);

        var database = configuration["Serilog:WriteTo:1:Args:databaseUrl"];
        var collectionName = configuration["Serilog:WriteTo:1:Args:logCollectionName"];

        using var db = new LiteDatabase(database);
        var collection = db.GetCollection(collectionName);
        return string.Join('\n', collection.FindAll().Select(l => l.ToString()));
    }
}