using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace Mother_of_Ping_CLI
{
    class tools
    {
        public static string[] pingStatusTable = new string[] { "Online", "Timed out", "Unreachable", "TTL expired", "General failure" };
        private static readonly Dictionary<IPStatus, int> ipStatusToPingStatus = new Dictionary<IPStatus, int>()
        {
            {IPStatus.BadDestination, 4},
            {IPStatus.BadHeader, 4},
            {IPStatus.BadOption, 4},
            {IPStatus.BadRoute, 4},
            {IPStatus.DestinationHostUnreachable, 2},
            {IPStatus.DestinationNetworkUnreachable, 2},
            {IPStatus.DestinationPortUnreachable, 2},
            //{IPStatus.DestinationProhibited, 2}, // DestinationProhibited & DestinationProtocolUnreachable have the same value
            {IPStatus.DestinationProtocolUnreachable, 2},
            {IPStatus.DestinationScopeMismatch, 2},
            {IPStatus.DestinationUnreachable, 2},
            {IPStatus.HardwareError, 4},
            {IPStatus.IcmpError, 4},
            {IPStatus.NoResources, 4},
            {IPStatus.PacketTooBig, 4},
            {IPStatus.ParameterProblem, 4},
            {IPStatus.SourceQuench, 4},
            {IPStatus.Success, 0},
            {IPStatus.TimedOut, 1},
            {IPStatus.TimeExceeded, 3},
            {IPStatus.TtlExpired, 3},
            {IPStatus.TtlReassemblyTimeExceeded, 3},
            {IPStatus.Unknown, 4},
            {IPStatus.UnrecognizedNextHeader, 4}
        };

        /// <summary>
        /// Pings the target
        /// </summary>
        /// <param name="ipAddr">Target victim, defaultIP only</param>
        /// <param name="timeout">Timeout limit in ms</param>
        /// <param name="limitRate">Cap the rate down to 1 per second</param>
        /// <param name="replyTime">Output Roundtrip Time</param>
        public static int ping(string ipAddr, int timeout, int bufferSize, int ttl, Boolean limitRate, out string replyAddr, out long replyTime, out int replyTtl)
        {
            int pingResult = ping(ipAddr, timeout, bufferSize, ttl, out replyAddr, out replyTime, out replyTtl);

            // Reduce ping rate by delaying more if roundtrip time is less than 1000ms
            if (limitRate && replyTime < 1000) Thread.Sleep((int)(1000 - replyTime));

            return pingResult;
        }

        public static int ping(string ipAddr, int timeout, int bufferSize, int ttl, out string replyAddr, out long replyTime, out int replyTtl)
        {
            // Parse victim address. Failback to 0.0.0.0 if any error occurs (will result in "General failure").
            IPAddress IP;
            string failbackIp = "0.0.0.0";
            try
            {
                IP = IPAddress.Parse(ipAddr);
            }
            catch
            {
                IP = IPAddress.Parse(failbackIp);
            }

            // Create a new instant
            Ping pingSender = new Ping();

            // Create a buffer of <bufferSize> bytes of data to be transmitted.
            byte[] buffer = new byte[bufferSize];
            //Random rnd = new Random();
            //rnd.NextBytes(buffer);

            // Set options for transmission:
            // The data can go through <ttl> gateways or routers before it is destroyed, and the data packet cannot be fragmented.
            PingOptions options = new PingOptions(ttl, true);

            // True work here
            PingReply reply = pingSender.Send(IP, timeout, buffer, options);

            // Now compiling result
            int pingResult = ipStatusToPingStatus[reply.Status];
            replyAddr = reply.Address.ToString();
            replyTime = (reply.Status == IPStatus.Success) ? reply.RoundtripTime : 0;
            replyTtl = reply.Options.Ttl;

            return pingResult;
        }
    }
}
