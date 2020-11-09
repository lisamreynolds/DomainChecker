using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

        Dictionary<AvailableServices, Func<string, object>> serviceCalls = new Dictionary<AvailableServices, Func<string, object>>()
        {
            { AvailableServices.ping, Services.Ping },
            { AvailableServices.test, endpoint => "test reply" }
        };

        // GET api/aggregate/[ip|domain]?services=[name],&services=[name]
        [HttpGet("{endpoint}")]
        public ActionResult<IEnumerable<object>> Get(string endpoint, AvailableServices[] services)
        {
            if (services.Length == 0) services = new AvailableServices[] { AvailableServices.ping };

            // Consider: .Distinct()
            return services.Select(service => serviceCalls[service].Invoke(endpoint)).ToList();
        }

    }
}
