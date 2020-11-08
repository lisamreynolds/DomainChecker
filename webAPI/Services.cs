using System;
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
            try
            {
                PingReply reply = pingSender.Send(endpoint, timeout, buffer, options);
                return reply.Status == IPStatus.Success ? "success" : "failure";
            } catch (Exception e)
            {
                return "An error occurred: " + e.InnerException.Message;
            }
        }
    }
}
