using LiteDB;
using Microsoft.AspNetCore.Mvc;

namespace TimeTeller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IConfiguration configuration;

    public AdminController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpGet("logs")]
    public string GetLogs()
    {
        var database = configuration["Serilog:WriteTo:1:Args:databaseUrl"];
        var collectionName = configuration["Serilog:WriteTo:1:Args:logCollectionName"];

        using var db = new LiteDatabase(database);
        var collection = db.GetCollection(collectionName);
        return string.Join('\n', collection.FindAll().Select(l => l.ToString()));
    }
}