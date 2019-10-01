using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Collections.Concurrent;

namespace Mother_of_Ping_CLI
{
    class pingWork
    {
        public pingWork()
        {
            resetStat();
        }

        static pingWork()
        {
            globalLog = new ConcurrentQueue<string[]>();
        }

        public int id { get; set; }
        public string hostname { get; set; }
        public string description { get; set; }
        public int period { get; set; }
        public int timeout { get; set; }
        public int bufferSize { get; set; }
        public int ttl { get; set; }
        public bool threadIsWorking { get; set; }
        public DateTime threadLastActiveTimestamp { get; private set; }

        public int totalCount { get; private set; }
        public int downCount { get; private set; }
        public int upCount { get; private set; }
        public int consecutiveDownCount { get; private set; }
        public int maxConsecutiveDownCount { get; private set; }
        private DateTime lastDownTimestampStart;
        public DateTime maxConsecutiveDownTimestampStart { get; private set; }
        public DateTime maxConsecutiveDownTimestampEnd { get; private set; }
        public TimeSpan maxConsecutiveDownDuration { get; private set; }
        public string percentDown { get; private set; }
        public pingStatus lastReply_result { get; private set; }
        public string lastReply_address { get; private set; }
        public long lastReply_time { get; private set; }
        public int lastReply_ttl { get; private set; }
        public DateTime lastReply_timestamp { get; private set; }
        public float avgPingTime { get; private set; }
        public long minPingTime { get; private set; }
        public long maxPingTime { get; private set; }
        public DateTime lastUpTimestamp { get; private set; }
        public DateTime lastDownTimestamp { get; private set; }
        public TimeSpan lastDownDuration { get; private set; }
        public int order { get { return id; } }

        private bool stopSignal = false;
        private Thread thread;

        public ConcurrentQueue<string[]> log;
        public static ConcurrentQueue<string[]> globalLog;

        public int latestLogSizeLimit { get; set; }
        public ConcurrentQueue<string[]> latestLog_all;
        public ConcurrentQueue<string[]> latestLog_down;

        Mutex mutex = new Mutex();

        public enum pingStatus
        {
            online = 0,
            timeout = 1,
            unreachable = 2,
            ttlExpired = 3,
            generalFailure = 4,
            invalid = -1,
            none = -2,
        }

        public static readonly Dictionary<pingStatus, string> pingStatusToText = new Dictionary<pingStatus, string>()
        {
            { pingStatus.online, "Online" },
            { pingStatus.timeout, "Timed out" },
            { pingStatus.unreachable, "Unreachable" },
            { pingStatus.ttlExpired, "TTL expired" },
            { pingStatus.generalFailure, "General failure" },
            { pingStatus.invalid, "Invalid" },
            { pingStatus.none, string.Empty },
        };

        public static readonly Dictionary<string, pingStatus> textToPingStatus = new Dictionary<string, pingStatus>()
        {
            { "Online", pingStatus.online },
            { "Timed out", pingStatus.timeout },
            { "Unreachable", pingStatus.unreachable },
            { "TTL expired", pingStatus.ttlExpired },
            { "General failure", pingStatus.generalFailure },
            { "Invalid", pingStatus.invalid },
            { string.Empty, pingStatus.none },
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
            // The data can go through <ttl> gateways or routers before it is destroyed, and the data packet can be fragmented.
            PingOptions options = new PingOptions(ttl, false);

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
            mutex.WaitOne();

            DateTime startLine = DateTime.Now;
            long timeOffset = 0;

            while (!stopSignal)
            {
                threadIsWorking = true;

                threadLastActiveTimestamp = DateTime.Now;
                lastReply_result = singlePing(hostname, timeout, bufferSize, ttl, out string replyAddr, out long replyTime, out int replyTtl);
                lastReply_address = replyAddr;
                lastReply_time = replyTime;
                lastReply_ttl = replyTtl;
                lastReply_timestamp = DateTime.Now;
                // Console.WriteLine(tools.pingStatusTable[result] + " " + replyAddr + " " + replyTime.ToString() + " " + replyTtl.ToString());

                // stat update
                updateStat();

                // create log line
                addLastPingToLog();

                // calculate when should this loop ends from the reference start time: <start> + <count> * <period>
                // sleep is not accurate, i.e may wake a few ms earlier or later than the requested span
                // because the allocated CPU is shared among several threads
                // so it might be unavailable or scheduled for others at the right time
                // Windows is not real-time OS, so never expect timing to be precise
                // the resolution is somewhere around 10ms
                // it isn't guaranteed to do much of anything except 'approximately' this long
                DateTime loopEndTime = startLine.AddMilliseconds(totalCount * period + timeOffset);
                int sleepTime = (int)(loopEndTime - DateTime.Now).TotalMilliseconds;

                // added timeOffset to avoid the situation where the loop runs without any delay
                // when the timeout > period, and timeout occurs, so sleepTime < 0 for (timeout - period) loop
                while (sleepTime <= 0)
                {
                    timeOffset += period;
                    loopEndTime = startLine.AddMilliseconds(totalCount * period + timeOffset);
                    sleepTime = (int)(loopEndTime - DateTime.Now).TotalMilliseconds;
                }

                Thread.Sleep(sleepTime);
            }

            //Console.WriteLine(hostname + ": exiting...");
            Console.WriteLine(hostname + ": " + totalCount.ToString() + " total, " + upCount.ToString() + " up, " + downCount.ToString() + " down");

            mutex.ReleaseMutex();
            threadIsWorking = false;
        }

        private void addLastPingToLog()
        {
            // log format: 0<hostname>,1<timestamp>,2<result>,3<round trip in ms>,4<TTL>,5<consecutiveDownCount>,6<check me>
            string[] line = new string[7] {
                hostname,
                tools.formatDateTime(lastReply_timestamp),
                pingStatusToText[lastReply_result],
                lastReply_time.ToString(),
                lastReply_ttl.ToString(),
                consecutiveDownCount.ToString(),
                (consecutiveDownCount >= 5) ? "x" : string.Empty
            };

            log.Enqueue(line);
            globalLog.Enqueue(line);

            latestLog_all.Enqueue(line);
            tools.trimConcurrentQueue(latestLog_all, latestLogSizeLimit);

            if (lastReply_result != pingStatus.online)
            {
                latestLog_down.Enqueue(line);
                tools.trimConcurrentQueue(latestLog_down, latestLogSizeLimit);
            }
        }

        private void updateStat()
        {
            totalCount++;
            if (lastReply_result == pingStatus.online)
            {
                upCount++;
                consecutiveDownCount = 0;
                lastUpTimestamp = lastReply_timestamp;

                avgPingTime = ((avgPingTime * (upCount - 1)) + lastReply_time) / upCount;
                if (lastReply_time > maxPingTime || upCount == 1)
                {
                    maxPingTime = lastReply_time;
                }
                if (lastReply_time < minPingTime || upCount == 1)
                {
                    minPingTime = lastReply_time;
                }
            }
            else
            {
                downCount++;
                consecutiveDownCount++;
                lastDownTimestamp = lastReply_timestamp;

                if (consecutiveDownCount == 1)
                {
                    lastDownTimestampStart = threadLastActiveTimestamp;
                }

                lastDownDuration = tools.roundTimeSpanToSecond(lastDownTimestamp - lastDownTimestampStart);

                if (consecutiveDownCount >= maxConsecutiveDownCount)
                {
                    maxConsecutiveDownCount = consecutiveDownCount;
                    maxConsecutiveDownTimestampEnd = lastReply_timestamp;
                    maxConsecutiveDownTimestampStart = lastDownTimestampStart;
                    maxConsecutiveDownDuration = tools.roundTimeSpanToSecond(maxConsecutiveDownTimestampEnd - maxConsecutiveDownTimestampStart);
                }
            }

            percentDown = string.Format("{0:0.##}%", (100.0 * downCount / totalCount));
        }

        public void resetStat()
        {
            maxConsecutiveDownCount = 0;
            maxConsecutiveDownTimestampStart = DateTime.MinValue;
            maxConsecutiveDownTimestampEnd = DateTime.MinValue;
            maxConsecutiveDownDuration = TimeSpan.MinValue;
            percentDown = string.Empty;
            lastReply_result = pingStatus.none;
            lastReply_address = string.Empty;
            lastReply_time = -1;
            lastReply_ttl = -1;
            avgPingTime = -1;
            minPingTime = -1;
            maxPingTime = -1;
            lastUpTimestamp = DateTime.MinValue;
            lastDownTimestamp = DateTime.MinValue;
            lastDownDuration = TimeSpan.MinValue;

            totalCount = 0;
            downCount = 0;
            upCount = 0;
            consecutiveDownCount = 0;

            log = new ConcurrentQueue<string[]>();
            latestLog_all = new ConcurrentQueue<string[]>();
            latestLog_down = new ConcurrentQueue<string[]>();

            if (latestLogSizeLimit < 10)
            {
                latestLogSizeLimit = 10; // enforce the minimum size
            }
        }

        public void startPing()
        {
            stopPingWait();
            stopSignal = false;
            thread = new Thread(() => this.backgroundPing());
            thread.Start();
        }

        public void stopPing()
        {
            stopSignal = true;
        }

        public void stopPingWait()
        {
            stopPing();
            mutex.WaitOne();
            mutex.ReleaseMutex();
        }
    }
}
