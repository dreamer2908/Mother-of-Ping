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
        public int id { get; set; }
        public string hostname { get; set; }
        public string description { get; set; }
        public int period { get; set; }
        public int timeout { get; set; }
        public int bufferSize { get; set; }
        public int ttl { get; set; }
        public bool threadIsWorking { get; set; }
        public bool flushLogSignal { get; set; }

        public int totalCount { get; private set; }
        public int downCount { get; private set; }
        public int upCount { get; private set; }
        public int consecutiveDownCount { get; private set; }
        public int maxConsecutiveDownCount { get; private set; }
        public string maxConsecutiveDownTimestamp { get; private set; }
        public string percentDown { get; private set; }
        public pingStatus lastReply_result { get; private set; }
        public string lastReply_address { get; private set; }
        public long lastReply_time { get; private set; }
        public int lastReply_ttl { get; private set; }
        public float avgPingTime { get; private set; }
        public long minPingTime { get; private set; }
        public long maxPingTime { get; private set; }
        public string lastUpTimestamp { get; private set; }
        public string lastDownTimestamp { get; private set; }
        public int order { get { return id; } }

        private bool stopSignal = false;
        private Thread thread;

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
        public static pingStatus singlePing(string hostname, int timeout, int bufferSize, int ttl, out string replyAddr, out long replyTime, out int replyTtl, Boolean limitRate, int period)
        {
            pingStatus pingResult = singlePing(hostname, timeout, bufferSize, ttl, out replyAddr, out replyTime, out replyTtl);

            // Reduce ping rate by delaying more if roundtrip time is less than 1000ms
            if (limitRate && replyTime < 1000) Thread.Sleep((int)(1000 - replyTime));

            return pingResult;
        }

        public static pingStatus singlePing(string hostname, int timeout, int bufferSize, int ttl, out string replyAddr, out long replyTime, out int replyTtl)
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

        private void backgroundPing()
        {
            while (!stopSignal)
            {
                threadIsWorking = true;

                // measure how much time ping and other works take
                Stopwatch watch = Stopwatch.StartNew();

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                lastReply_result = singlePing(hostname, timeout, bufferSize, ttl, out string replyAddr, out long replyTime, out int replyTtl);
                lastReply_address = replyAddr;
                lastReply_time = replyTime;
                lastReply_ttl = replyTtl;
                // Console.WriteLine(tools.pingStatusTable[result] + " " + replyAddr + " " + replyTime.ToString() + " " + replyTtl.ToString());

                // stat update
                totalCount++;
                if (lastReply_result == pingStatus.online)
                {
                    upCount++;
                    consecutiveDownCount = 0;
                    lastUpTimestamp = timestamp;
                    avgPingTime = ((avgPingTime * (upCount - 1)) + lastReply_time) / upCount;
                }
                else
                {
                    downCount++;
                    consecutiveDownCount++;
                    lastDownTimestamp = timestamp;

                    if (consecutiveDownCount >= maxConsecutiveDownCount)
                    {
                        maxConsecutiveDownCount = consecutiveDownCount;
                        maxConsecutiveDownTimestamp = timestamp;
                    }
                }

                percentDown = string.Format("{0:0.##}%", ((float)downCount / totalCount));

                if (lastReply_time > maxPingTime)
                    maxPingTime = lastReply_time;
                if (lastReply_time < minPingTime)
                    minPingTime = lastReply_time;

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

        public void resetStat()
        {
            maxConsecutiveDownCount = 0;
            maxConsecutiveDownTimestamp = string.Empty;
            percentDown = string.Empty;
            lastReply_result = pingStatus.online;
            lastReply_address = string.Empty;
            lastReply_time = -1;
            lastReply_ttl = -1;
            avgPingTime = -1;
            minPingTime = -1;
            maxPingTime = -1;
            lastUpTimestamp = string.Empty;
            lastDownTimestamp = string.Empty;

            totalCount = 0;
            downCount = 0;
            upCount = 0;
            consecutiveDownCount = 0;
        }

        public void startPing()
        {
            thread = new Thread(() => this.backgroundPing());
            thread.Start();
        }

        public void stopPing()
        {
            stopSignal = true;
        }
    }
}
