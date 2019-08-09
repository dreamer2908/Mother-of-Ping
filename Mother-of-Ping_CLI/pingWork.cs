using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace Mother_of_Ping_CLI
{
    class pingWork
    {
        public enum pingStatus
        {
            online = 0,
            timeout = 1,
            unreachable = 2,
            ttlExpired = 3,
            generalFailure = 4
        }

        public static readonly Dictionary<pingStatus, string> pingStatusToText = new Dictionary<pingStatus, string>()
        {
            { pingStatus.online, "Online" },
            { pingStatus.timeout, "Timed out" },
            { pingStatus.unreachable, "Unreachable" },
            { pingStatus.ttlExpired, "TTL expired" },
            { pingStatus.generalFailure, "General failure" },
        };

        private static readonly Dictionary<IPStatus, pingStatus> ipStatusToPingStatus = new Dictionary<IPStatus, pingStatus>()
        {
            {IPStatus.BadDestination, pingStatus.generalFailure},
            {IPStatus.BadHeader, pingStatus.generalFailure},
            {IPStatus.BadOption, pingStatus.generalFailure},
            {IPStatus.BadRoute, pingStatus.generalFailure},
            {IPStatus.DestinationHostUnreachable, pingStatus.unreachable},
            {IPStatus.DestinationNetworkUnreachable, pingStatus.unreachable},
            {IPStatus.DestinationPortUnreachable, pingStatus.unreachable},
            //{IPStatus.DestinationProhibited, pingStatus.unreachable}, // DestinationProhibited & DestinationProtocolUnreachable have the same value
            {IPStatus.DestinationProtocolUnreachable, pingStatus.unreachable},
            {IPStatus.DestinationScopeMismatch, pingStatus.unreachable},
            {IPStatus.DestinationUnreachable, pingStatus.unreachable},
            {IPStatus.HardwareError, pingStatus.generalFailure},
            {IPStatus.IcmpError, pingStatus.generalFailure},
            {IPStatus.NoResources, pingStatus.generalFailure},
            {IPStatus.PacketTooBig, pingStatus.generalFailure},
            {IPStatus.ParameterProblem, pingStatus.generalFailure},
            {IPStatus.SourceQuench, pingStatus.generalFailure},
            {IPStatus.Success, pingStatus.online},
            {IPStatus.TimedOut, pingStatus.timeout},
            {IPStatus.TimeExceeded, pingStatus.ttlExpired},
            {IPStatus.TtlExpired, pingStatus.ttlExpired},
            {IPStatus.TtlReassemblyTimeExceeded, pingStatus.ttlExpired},
            {IPStatus.Unknown, pingStatus.generalFailure},
            {IPStatus.UnrecognizedNextHeader, pingStatus.generalFailure}
        };

        /// <summary>
        /// Pings the target
        /// </summary>
        /// <param name="hostname">Target victim, either IP address or hostname</param>
        /// <param name="timeout">Timeout limit in ms</param>
        /// <param name="limitRate">Cap the rate down to 1 per second</param>
        /// <param name="replyTime">Output Roundtrip Time</param>
        public static pingStatus ping(string hostname, int timeout, int bufferSize, int ttl, out string replyAddr, out long replyTime, out int replyTtl, Boolean limitRate, int period)
        {
            pingStatus pingResult = ping(hostname, timeout, bufferSize, ttl, out replyAddr, out replyTime, out replyTtl);

            // Reduce ping rate by delaying more if roundtrip time is less than 1000ms
            if (limitRate && replyTime < 1000) Thread.Sleep((int)(1000 - replyTime));

            return pingResult;
        }

        public static pingStatus ping(string hostname, int timeout, int bufferSize, int ttl, out string replyAddr, out long replyTime, out int replyTtl)
        {
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
            PingReply reply = null;
            pingStatus pingResult;

            try
            {
                reply = pingSender.Send(hostname, timeout, buffer, options);
            }
            catch (System.Net.NetworkInformation.PingException)
            {
            }

            // Now compiling result
            pingResult = (reply != null) ? ipStatusToPingStatus[reply.Status] : pingStatus.generalFailure;
            replyAddr = (reply != null && reply.Address != null) ? reply.Address.ToString() : string.Empty; // address is null when timeout
            replyTime = (reply != null && reply.Status == IPStatus.Success) ? reply.RoundtripTime : 0;
            replyTtl = (reply != null && reply.Options != null) ? reply.Options.Ttl : 0; // options is null when unreachable

            return pingResult;
        }

        public static void backgroundPing(string hostname, int period, int timeout, int bufferSize, int ttl, ref bool threadAlive, ref bool flushLogSignal, ref bool continueMonitor, ref string actualHost)
        {
            actualHost = hostname;

            int totalCount = 0;
            int downCount = 0;
            int upCount = 0;
            int consecutiveDownCount = 0;

            while (continueMonitor)
            {
                threadAlive = true;

                // measure how much time ping and other works take
                Stopwatch watch = Stopwatch.StartNew();

                pingStatus result = ping(hostname, timeout, bufferSize, ttl, out string replyAddr, out long replyTime, out int replyTtl);
                // Console.WriteLine(tools.pingStatusTable[result] + " " + replyAddr + " " + replyTime.ToString() + " " + replyTtl.ToString());
                totalCount++;
                if (result == pingStatus.online)
                {
                    upCount++;
                    consecutiveDownCount = 0;
                }
                else
                {
                    downCount++;
                    consecutiveDownCount++;
                }

                // todo: create log line

                if (flushLogSignal)
                {
                    flushLogSignal = false;
                    // todo: flush log
                }

                // then sleep for <period> - <time taken>
                watch.Stop();
                int elapsedMs = (int)watch.ElapsedMilliseconds;

                //if (elapsedMs > timeout)
                //{
                //    Console.WriteLine(hostname + " missed: " + elapsedMs.ToString());
                //}

                int sleepTime = period - elapsedMs;
                if (sleepTime > 0)
                {
                    Thread.Sleep(sleepTime);
                }
                //else
                //{
                //    Console.WriteLine(hostname + " will NOT sleep: " + sleepTime.ToString());
                //}
            }

            // todo: flush log

            //Console.WriteLine(hostname + ": exiting...");
            Console.WriteLine(hostname + ": " + totalCount.ToString() + " total, " + upCount.ToString() + " up, " + downCount.ToString() + " down");

        }
    }
}
