using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
            geolocation
        }

        Dictionary<AvailableServices, Func<string, Task<string>>> serviceCalls = new Dictionary<AvailableServices, Func<string, Task<string>>>()
        {
            { AvailableServices.ping, PingAsync },
            { AvailableServices.geolocation, GeolocationAsync }
        };

        // GET api/aggregate/[ip|domain]?services=[name],&services=[name]
        [HttpGet("{endpoint}")]
        public ActionResult<IEnumerable<object>> Get(string endpoint, [FromQuery] AvailableServices[] services)
        {
            if (services.Length == 0) services = new AvailableServices[] { AvailableServices.ping };

            // Consider: .Distinct()
            var serviceTasks = services.Select(service => serviceCalls[service].Invoke(endpoint));
            Task.WhenAll(serviceTasks).Wait();
            return serviceTasks.Select(task => task.Result).ToList();
        }

        private static async Task<string> PingAsync(string endpoint)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress(PING_ADDRESS);
                var client = new GRPCPing.GRPCPingClient(channel);
                var reply = await client.UnaryCallAsync(new GRPCPingRequest { Endpoint = endpoint }, deadline: DateTime.UtcNow.AddSeconds(5));

                return reply.Message;
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.DeadlineExceeded)
            {
                return "Ping timeout.";
            }
        }

        private static async Task<string> GeolocationAsync(string endpoint)
        {
            try
            {
                using var channel = GrpcChannel.ForAddress(GEOLOCATION_ADDRESS);
                var client = new GRPCGeolocation.GRPCGeolocationClient(channel);
                var reply = await client.UnaryCallAsync(new GRPCGeolocationRequest { Endpoint = endpoint }, deadline: DateTime.UtcNow.AddSeconds(5));

                return reply.Message;
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.DeadlineExceeded)
            {
                return "Geolocation timeout.";
            }
        }
    }
}
