using Microsoft.AspNetCore.Mvc;

namespace webAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregateController : Controller
    {
        // GET api/aggregate/[ip|domain]
        [HttpGet("{endpoint}")]
        public ActionResult<string> Get(string endpoint)
        {
            return Services.Ping(endpoint);
        }
    }
}
