using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PingService
{
    public class PingService : GRPCPing.GRPCPingBase
    {
        private readonly ILogger<PingService> _logger;
        public PingService(ILogger<PingService> logger)
        {
            _logger = logger;
        }

        public override Task<GRPCPingReply> UnaryCall(GRPCPingRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GRPCPingReply
            {
                Message = SendPing(request.Endpoint)
            });
        }

        string SendPing(string endpoint)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions
            {
                DontFragment = true
            };

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120; // ms

            PingReply reply = pingSender.Send(endpoint, timeout, buffer, options);
            return reply.Status == IPStatus.Success ? "ping successful" : "ping failed";
        }
    }
}
