using LiteDB;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace TimeTeller.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    [HttpGet("logs")]
    public IActionResult GetLogs()
    {
        var result = new StringBuilder();

        using var db = new LiteDatabase("Log.db");
        var collection = db.GetCollection("DefaultLog");
        foreach (BsonDocument logEntry in collection.FindAll()) result.Append(logEntry.ToString());

        var response = new OkObjectResult(result.ToString());
        response.ContentTypes.Add("application/json");
        return response;
    }
}