using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mother_of_Ping_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            string ipAddr = "10.30.11.110";
            int timeout = 5000;
            int bufferSize = 32;
            int ttl = 128;
            bool wait1s = true;

            for (int i = 0; i < 100; i++)
            {
                int result = tools.ping(ipAddr, timeout, bufferSize, ttl, wait1s, out string replyAddr, out long replyTime, out int replyTtl);
                Console.WriteLine(tools.pingStatusTable[result] + " " + replyAddr + " " + replyTime.ToString() + " " + replyTtl.ToString());
            }

        }
    }
}
