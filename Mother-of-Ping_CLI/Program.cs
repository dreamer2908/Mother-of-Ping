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
                work.description = hostList[i][1];
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

            Thread.Sleep(3000);

            pingWork worker = workForce[0];

            tools.printVariableNameAndValue(nameof(worker.id), worker.id);
            tools.printVariableNameAndValue(nameof(worker.hostname), worker.hostname);
            tools.printVariableNameAndValue(nameof(worker.description), worker.description);
            tools.printVariableNameAndValue(nameof(worker.period), worker.period);
            tools.printVariableNameAndValue(nameof(worker.timeout), worker.timeout);
            tools.printVariableNameAndValue(nameof(worker.bufferSize), worker.bufferSize);
            tools.printVariableNameAndValue(nameof(worker.ttl), worker.ttl);
            tools.printVariableNameAndValue(nameof(worker.threadIsWorking), worker.threadIsWorking);
            tools.printVariableNameAndValue(nameof(worker.totalCount), worker.totalCount);
            tools.printVariableNameAndValue(nameof(worker.downCount), worker.downCount);
            tools.printVariableNameAndValue(nameof(worker.upCount), worker.upCount);
            tools.printVariableNameAndValue(nameof(worker.consecutiveDownCount), worker.consecutiveDownCount);
            tools.printVariableNameAndValue(nameof(worker.maxConsecutiveDownCount), worker.maxConsecutiveDownCount);
            tools.printVariableNameAndValue(nameof(worker.maxConsecutiveDownTimestampEnd), worker.maxConsecutiveDownTimestampEnd);
            tools.printVariableNameAndValue(nameof(worker.percentDown), worker.percentDown);
            tools.printVariableNameAndValue(nameof(worker.lastReply_result), worker.lastReply_result);
            tools.printVariableNameAndValue(nameof(worker.lastReply_address), worker.lastReply_address);
            tools.printVariableNameAndValue(nameof(worker.lastReply_time), worker.lastReply_time);
            tools.printVariableNameAndValue(nameof(worker.lastReply_ttl), worker.lastReply_ttl);
            tools.printVariableNameAndValue(nameof(worker.avgPingTime), worker.avgPingTime);
            tools.printVariableNameAndValue(nameof(worker.minPingTime), worker.minPingTime);
            tools.printVariableNameAndValue(nameof(worker.maxPingTime), worker.maxPingTime);
            tools.printVariableNameAndValue(nameof(worker.lastUpTimestamp), worker.lastUpTimestamp);
            tools.printVariableNameAndValue(nameof(worker.lastDownTimestamp), worker.lastDownTimestamp);
            tools.printVariableNameAndValue(nameof(worker.order), worker.order);

            int logCount = worker.log.Count;
            for (int i = 0; i < logCount; i++)
            {
                if (worker.log.TryDequeue(out string[] logLine))
                {
                    tools.printObjectArray((object[])logLine);
                }
            }

            tools.writeCsv_ConcurrentQueue(pingWork.globalLog, "globalLog.csv", true);

            int globalLogCount = pingWork.globalLog.Count;
            Console.WriteLine("\n\nGlobal log: {0}", globalLogCount);

            for (int i = 0; i < globalLogCount; i++)
            {
                if (pingWork.globalLog.TryDequeue(out string[] logLine))
                {
                    Console.WriteLine("=== {0} ===", i);
                    tools.printObjectArray((object[])logLine);
                }
            }

            tools.generateCsvReport(workForce, "session_report.csv");
            tools.generateHtmlReport(workForce, "session_report.html");

            Console.ReadLine();
        }
    }
}
