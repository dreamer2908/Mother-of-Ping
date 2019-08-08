using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

namespace Mother_of_Ping_CLI
{
    class tools
    {
        #region misc
        private static void sanitizeStringArray(string[] elements)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] == null)
                    elements[i] = string.Empty;

                elements[i] = elements[i].Trim();
            }
        }

        private static string[] sanitizeStringArray(string[] fields, int numberOfElements)
        {
            // make sure it contains exactly <numberOfElements> elements
            string[] elements = new string[numberOfElements];

            for (int i = 0; i < fields.Length && i < numberOfElements; i++)
                elements[i] = fields[i];

            // and they're not null
            sanitizeStringArray(elements);

            return elements;
        }

        public static void sanitizeBoolArray(bool[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
                arr[i] = false;
        }

        public static int countValueBoolArray(bool[] arr, bool value)
        {
            int count = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == value)
                    count++;
            }

            return count;
        }
        #endregion

        #region printer
        public static void printListOfStringArray(List<string[]> l)
        {
            for (int i = 0; i < l.Count; i++)
            {
                string[] arr = l[i];
                Console.WriteLine(i.ToString() + ":");
                foreach (string s in arr)
                {
                    Console.WriteLine("    " + s);
                }
            }
        }

        public static void printBoolArray(bool[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                bool s = arr[i];
                Console.WriteLine(i.ToString() + ": " + s.ToString());
            }
        }

        public static void printObjectArray(object[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                object s = arr[i];
                Console.WriteLine(i.ToString() + ": " + s.ToString());
            }
        }
        #endregion

        #region parsers
        public static List<string[]> txtPinginfoviewParser(string textFile, bool hasHeader)
        {
            List<string[]> result = new List<string[]>();

            string[] lines = File.ReadAllLines(textFile);

            int startLine = hasHeader ? 1 : 0;
            for (int i = startLine; i < lines.Length; i++)
            {
                string s = lines[i].Trim();
                int numberOfElements = 2;
                string[] fields = s.Split(new string[] { " " }, numberOfElements, StringSplitOptions.None);

                string[] elements = sanitizeStringArray(fields, numberOfElements);

                if (elements[0].Length > 0)
                    result.Add(elements);
            }

            return result;
        }

        public static List<string[]> csvHostListParser(string textFile, bool hasHeader)
        {
            List<string[]> result = new List<string[]>();

            using (TextFieldParser parser = new TextFieldParser(textFile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                if (hasHeader) // skip the header if available
                    parser.ReadLine();

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    int numberOfElements = 2;

                    string[] elements = sanitizeStringArray(fields, numberOfElements);

                    if (elements[0].Length > 0)
                        result.Add(elements);
                }
            }
            return result;
        }

        #endregion

        #region ping work
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
        public static int ping(string ipAddr, int timeout, int bufferSize, int ttl, out string replyAddr, out long replyTime, out int replyTtl, Boolean limitRate, int period)
        {
            int pingResult = ping(ipAddr, timeout, bufferSize, ttl, out replyAddr, out replyTime, out replyTtl);

            // Reduce ping rate by delaying more if roundtrip time is less than 1000ms
            if (limitRate && replyTime < 1000) Thread.Sleep((int)(1000 - replyTime));

            return pingResult;
        }

        public static int ping(string ipAddr, int timeout, int bufferSize, int ttl, out string replyAddr, out long replyTime, out int replyTtl)
        {
            // Parse victim address
            IPAddress IP;
            try
            {
                IP = IPAddress.Parse(ipAddr);
            }
            catch
            {
                replyAddr = string.Empty;
                replyTime = 0;
                replyTtl = 0;
                return 4;
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
            replyAddr = (reply.Address != null) ? reply.Address.ToString(): string.Empty; // it's null when timeout
            replyTime = (reply.Status == IPStatus.Success) ? reply.RoundtripTime : 0;
            replyTtl = (reply.Options != null) ? reply.Options.Ttl : 0; // when unreachable, it's null

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

                int result = ping(hostname, timeout, bufferSize, ttl, out string replyAddr, out long replyTime, out int replyTtl);
                // Console.WriteLine(tools.pingStatusTable[result] + " " + replyAddr + " " + replyTime.ToString() + " " + replyTtl.ToString());
                totalCount++;
                if (result == 0)
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
                int elapsedMs = (int) watch.ElapsedMilliseconds;

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
        #endregion
    }
}
