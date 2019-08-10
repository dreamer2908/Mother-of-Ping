using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;
using System.Text;

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

        // From http://web.archive.org/web/20130124234247/http://abdullin.com/journal/2008/12/13/how-to-find-out-variable-or-parameter-name-in-c.html
        //public static void Check<T>(Expression<Func<T>> expr)
        //{
        //    var body = ((MemberExpression)expr.Body);
        //    Console.WriteLine("Name is: {0}", body.Member.Name);
        //    Console.WriteLine("Value is: {0}", ((FieldInfo)body.Member).GetValue(((ConstantExpression)body.Expression).Value));
        //}

        //public static void Check<T>(Func<T> expr)
        //{
        //    // get IL code behind the delegate
        //    var il = expr.Method.GetMethodBody().GetILAsByteArray();
        //    // bytes 2-6 represent the field handle
        //    var fieldHandle = BitConverter.ToInt32(il, 2);
        //    // resolve the handle
        //    var field = expr.Target.GetType()
        //      .Module.ResolveField(fieldHandle);

        //    Console.WriteLine("Name is: {0}", field.Name);
        //    Console.WriteLine("Value is: {0}", expr());
        //}

        public static void printVariableNameAndValue(string name, object value)
        {
            Console.Write(name);
            Console.Write(" = ");
            Console.WriteLine(value);
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

        public static void writeCsv_ConcurrentQueue(ConcurrentQueue<string[]> log, string filename, bool overwrite)
        {
            StringBuilder sb = new StringBuilder();
            int logCount = log.Count;
            for (int i = 0; i < logCount; i++)
            {
                if (log.TryDequeue(out string[] logLine))
                {
                    for (int j = 0; j < logLine.Length; j++)
                    {
                        logLine[j] = quoteStringForCsv(logLine[j]);
                    }
                    sb.AppendLine(string.Join(",", logLine));
                }
            }

            if (overwrite)
            {
                File.WriteAllText(filename, sb.ToString());
            }
            else
            {
                File.AppendAllText(filename, sb.ToString());
            }
        }

        // see https://en.m.wikipedia.org/wiki/Comma-separated_values for what to quote
        private static string quoteStringForCsv(string input)
        {
            string result = input;

            if (result.Contains(",") || result.Contains("\"") || result.Contains("\n") || result.Contains("\r"))
            {
                result = result.Replace("\"", "\"\"");
                result = "\"" + result + "\"";
            }

            return result;
        }

        #endregion

        #region reports
        public static void generateCsvReport(pingWork[] workForce, string filename)
        {
            ConcurrentQueue<string[]> contents = new ConcurrentQueue<string[]>();
            string[] header = new string[] {
                "Host Name", // 0
                "Description", // 1
                "Reply IP Address", // 2
                "Succeed Count", // 3
                "Failed Count", // 4
                "Consecutive Failed Count", // 5
                "Max Consecutive Failed Count", // 6
                "Max Consecutive Failed Time", // 7
                "% Failed", // 8
                "Last Ping Status", // 9
                "Last Ping Time", // 10
                "Average Ping Time", // 11
                "Last Succeed On", // 12
                "Last Failed On", // 13
                "Minimum Ping Time", // 14
                "Maximum Ping Time" // 15
            };
            contents.Enqueue(header);

            foreach (pingWork work in workForce)
            {
                string[] line = new string[] {
                    work.hostname, // 0
                    work.description, // 1
                    work.lastReply_address, // 2
                    work.upCount.ToString(), // 3
                    work.downCount.ToString(), // 4
                    work.consecutiveDownCount.ToString(), // 5
                    work.maxConsecutiveDownCount.ToString(), // 6
                    work.maxConsecutiveDownTimestamp, // 7
                    work.percentDown, // 8
                    pingWork.pingStatusToText[work.lastReply_result], // 9
                    work.lastReply_time.ToString(), // 10
                    string.Format("{0:0.#}", work.avgPingTime), // 11
                    work.lastUpTimestamp, // 12
                    work.lastDownTimestamp, // 13
                    work.minPingTime.ToString(), // 14
                    work.maxPingTime.ToString() // 15
                };
                contents.Enqueue(line);
            }

            writeCsv_ConcurrentQueue(contents, filename, true);
        }

        #endregion
    }
}
