using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeoLocationService
{
    public class GeolocationService : GRPCGeolocation.GRPCGeolocationBase
    {
        static readonly HttpClient client = new HttpClient();
        private readonly ILogger<GeolocationService> _logger;
    
        public GeolocationService(ILogger<GeolocationService> logger)
        {
            _logger = logger;
        }

        public override Task<GRPCGeolocationReply> UnaryCall(GRPCGeolocationRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GRPCGeolocationReply
            {
                Message = Geolocate(request.Endpoint)
            }); ;
        }

        string Geolocate(string endpoint)
        {
            // TODO: api key configuration
            string API_KEY = "***REMOVED***";
            string type = IPAddress.TryParse(endpoint, out _) ? "ipAddress" : "domain";

            string uri = $"https://ip-geolocation.whoisxmlapi.com/api/v1?apiKey={API_KEY}&{type}={endpoint}";
            try
            {
                return client.GetStringAsync(uri).Result;
            }
            catch (HttpRequestException e)
            {
                return e.Message;
            }
        }
    }
}
