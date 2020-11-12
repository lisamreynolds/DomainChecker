using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Grpc.Net.Client;

namespace webAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregateController : Controller
    {
        // Fun hardcoded URLS!
        // Knee-jerk solution: pretend these are actually load balancers and anything behind them can scale as needed
        // also TODO: these as configuration
        private const string PING_ADDRESS = "https://localhost:5001";
        private const string GEOLOCATION_ADDRESS = "https://localhost:5002";

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum AvailableServices
        {
            ping,
            geolocation,
            test
        }

        Dictionary<AvailableServices, Func<string, object>> serviceCalls = new Dictionary<AvailableServices, Func<string, object>>()
        {
            { AvailableServices.ping, Ping },
            { AvailableServices.geolocation, Geolocation },
            { AvailableServices.test, endpoint => "test reply" }
        };

        // GET api/aggregate/[ip|domain]?services=[name],&services=[name]
        [HttpGet("{endpoint}")]
        public ActionResult<IEnumerable<object>> Get(string endpoint, [FromQuery] AvailableServices[] services)
        {
            if (services.Length == 0) services = new AvailableServices[] { AvailableServices.ping };

            // Consider: .Distinct()
            return services.Select(service => serviceCalls[service].Invoke(endpoint)).ToList();
        }

        private static string Ping(string endpoint)
        {
            using var channel = GrpcChannel.ForAddress(PING_ADDRESS);
            var client = new GRPCPing.GRPCPingClient(channel);
            var reply = client.UnaryCall(new GRPCPingRequest { Endpoint = endpoint });

            return reply.Message;
        }

        private static string Geolocation(string endpoint)
        {
            using var channel = GrpcChannel.ForAddress(GEOLOCATION_ADDRESS);
            var client = new GRPCGeolocation.GRPCGeolocationClient(channel);
            var reply = client.UnaryCall(new GRPCGeolocationRequest { Endpoint = endpoint });

            return reply.Message;
        }
    }
}
