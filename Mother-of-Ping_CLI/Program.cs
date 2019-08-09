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
            pingWork[] workForce = new pingWork[numOfHost];

            Console.WriteLine("numOfHost: " + numOfHost.ToString());

            for (int i = 0; i < numOfHost; i++)
            {
                pingWork work = new pingWork();
                workForce[i] = work;

                work.id = i;
                work.hostname = hostList[i][0];
                work.period = 1000;
                work.timeout = 1000;
                work.bufferSize = 32;
                work.ttl = 128;

                work.startPing();
            }

            for (int i = 0; i < 30; i++)
            {
                Console.WriteLine("threadAliveSignal: ");
                bool[] threadAliveSignal = new bool[numOfHost];
                for (int j = 0; j < numOfHost; j++)
                {
                    threadAliveSignal[j] = workForce[j].threadIsWorking;
                    workForce[j].threadIsWorking = false;
                }

                //tools.printBoolArray(threadAliveSignal);
                Console.WriteLine("True: " + tools.countValueBoolArray(threadAliveSignal, true).ToString());
                Console.WriteLine("False: " + tools.countValueBoolArray(threadAliveSignal, false).ToString());

                for (int j = 0; j < numOfHost; j++)
                {
                    if (threadAliveSignal[j] == false)
                    {
                        Console.WriteLine("Missed #" + j.ToString() + " of " + hostList[j][0]);
                    }
                }

                Thread.Sleep(1000);
            }

            for (int i = 0; i < numOfHost; i++)
            {
                workForce[i].stopPing();
            }

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
