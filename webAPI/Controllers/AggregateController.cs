using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace webAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregateController : Controller
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum AvailableServices
        {
            ping,
            test
        }

        // GET api/aggregate/[ip|domain]?services=[name],&services=[name]
        [HttpGet("{endpoint}")]
        public ActionResult<string> Get(string endpoint, AvailableServices[] services)
        {
            if (services.Length == 0) services = new AvailableServices[] { AvailableServices.ping };

            string results = "";

            if (Array.Exists(services, s => s.Equals(AvailableServices.ping))) results += Services.Ping(endpoint);
            if (Array.Exists(services, s => s.Equals(AvailableServices.test))) results += "\n" + "test reply";

            return results;
        }
    }
}
