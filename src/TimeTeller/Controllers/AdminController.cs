using LiteDB;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace TimeTeller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [HttpGet("logs")]
        public ActionResult<string> Logs()
        {
            StringBuilder result = new StringBuilder();

            using (var db = new LiteDatabase("NLog.db"))
            {
                var col = db.GetCollection("DefaultLog");
                foreach (BsonDocument logEntry in col.FindAll()) result.Append(logEntry.ToString());
            }

            return result.ToString();
        }
    }
}