using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mother_of_Ping_CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //string ipAddr = "10.30.11.110";
            //int timeout = 5000;
            //int bufferSize = 32;
            //int ttl = 128;
            //bool wait1s = true;

            //for (int i = 0; i < 0; i++)
            //{
            //    int result = tools.ping(ipAddr, timeout, bufferSize, ttl, out string replyAddr, out long replyTime, out int replyTtl, wait1s, 1000);
            //    Console.WriteLine(tools.pingStatusTable[result] + " " + replyAddr + " " + replyTime.ToString() + " " + replyTtl.ToString());
            //}

            List<string[]> hostList = tools.csvHostListParser(@"D:\Code\MonitorHost\IP_List_2019-07-05 - test 1.csv", false);
            tools.printListOfStringArray(hostList);

            int numOfHost = hostList.Count;
            bool[] threadAliveSignal = new bool[numOfHost];
            bool[] fllushLogSignal = new bool[numOfHost];
            string[] actualHostList = new string[numOfHost];
            bool continueMonitor = true;

            Console.WriteLine("numOfHost: " + numOfHost.ToString());

            for (int i = 0; i < numOfHost; i++)
            {
                int threadId = i;
                string hostname = hostList[i][0];
                int period = 1000;
                int timeout = 1000;
                int bufferSize = 32;
                int ttl = 128;
                var thread = new Thread(() => tools.backgroundPing(hostname, period, timeout, bufferSize, ttl, ref threadAliveSignal[threadId], ref fllushLogSignal[threadId], ref continueMonitor, ref actualHostList[threadId]));
                thread.Start();
            }

            for (int i = 0; i < 30; i++)
            {
                Console.WriteLine("threadAliveSignal: ");
                //tools.printBoolArray(threadAliveSignal);
                Console.WriteLine("True: " + tools.countValueBoolArray(threadAliveSignal, true).ToString());
                Console.WriteLine("False: " + tools.countValueBoolArray(threadAliveSignal, false).ToString());

                for (int j = 0; j < numOfHost; j++)
                {
                    if (threadAliveSignal[j] == false)
                    {
                        Console.WriteLine("Missed #" + j.ToString() + " of " + actualHostList[j]);
                    }
                }

                tools.sanitizeBoolArray(threadAliveSignal);

                Thread.Sleep(1000);
            }

            continueMonitor = false;

            // check if the target threads received is the same as what main sended
            //for (int i = 0; i < numOfHost; i++)
            //{
            //    string target = hostList[i][0];
            //    string realTarget = actualHostList[i];
            //    if (target == realTarget)
            //    {
            //        Console.WriteLine("Host " + i.ToString() + " matches");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Host " + i.ToString() + " NOT matches");
            //    }
            //}

            Console.ReadLine();
        }
    }
}
