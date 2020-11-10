using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;

namespace webAPI
{
    /*
     * "Send those tasks to a pool of 2+ workers. The API must support workers on separate processes/machines/docker containers than the API."
     * Just throwing things here for now
     */
    public class Services
    {
        static readonly HttpClient client = new HttpClient();

        public static string Ping(string endpoint)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120; // ms
            
            PingReply reply = pingSender.Send(endpoint, timeout, buffer, options);
            return reply.Status == IPStatus.Success ? "ping successful" : "ping failed";
        }

        public static string Geolocation(string endpoint)
        {
            string API_KEY = "***REMOVED***";
            IPAddress address;
            string type = IPAddress.TryParse(endpoint, out address) ? "ipAddress" : "domain";

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
