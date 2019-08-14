using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;
using System.Text;
using System.Xml;

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

        private static string XmlEscape(string unescaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerXml;
        }

        private static string XmlUnescape(string escaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerXml = escaped;
            return node.InnerText;
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
            ConcurrentQueue<string[]> contents = prepareReport(workForce);

            writeCsv_ConcurrentQueue(contents, filename, true);
        }

        private static ConcurrentQueue<string[]> prepareReport(pingWork[] workForce)
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
                "Max Consecutive Failed Start Time", // 7
                "Max Consecutive Failed End Time", // 8
                "% Failed", // 9
                "Last Ping Status", // 10
                "Last Ping Time", // 11
                "Average Ping Time", // 12
                "Last Succeed On", // 13
                "Last Failed On", // 14
                "Minimum Ping Time", // 15
                "Maximum Ping Time" // 16
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
                    work.maxConsecutiveDownTimestampStart, // 7
                    work.maxConsecutiveDownTimestampEnd, // 8
                    work.percentDown, // 9
                    pingWork.pingStatusToText[work.lastReply_result], // 10
                    work.lastReply_time.ToString(), // 11
                    (work.upCount == 0) ? string.Empty : string.Format("{0:0.#}", work.avgPingTime), // 12
                    work.lastUpTimestamp, // 13
                    work.lastDownTimestamp, // 14
                    (work.upCount == 0) ? string.Empty : work.minPingTime.ToString(), // 15
                    (work.upCount == 0) ? string.Empty : work.maxPingTime.ToString() // 16
                };
                contents.Enqueue(line);
            }

            return contents;
        }

        public static void generateHtmlReport(pingWork[] workForce, string filename)
        {
            ConcurrentQueue<string[]> contents = prepareReport(workForce);

            writeHtmlReport(contents, filename);
        }

        private static void writeHtmlReport(ConcurrentQueue<string[]> contents, string filename)
        {
            StringBuilder sb = new StringBuilder();
            string part1 = @"<?xml version=""1.0"" encoding=""utf-8""?><!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd""><html xmlns=""http://www.w3.org/1999/xhtml""><head><title></title></head><body><h3>Pings List<br/></h3><h4>Created using Mother of Ping!<br/></h4><table border=""1"" cellpadding=""5""><tr style=""background-color: #E0E0E0;"">";
            sb.Append(part1);

            contents.TryDequeue(out string[] header);
            foreach (string s in header)
            {
                sb.Append("<th>" + XmlEscape(s) + "</th>");
            }

            sb.Append(@"</tr>");

            string[] row;
            while (contents.TryDequeue(out row))
            {
                sb.Append(@"<tr style=""background-color: #FFFFFF; white-space: nowrap;"">");
                foreach (string s in row)
                {
                    sb.Append("<td>" + XmlEscape(s) + "</td>");
                }
                sb.Append(@"</tr>");
            }

            sb.Append(@"</table></body></html>");

            Encoding utf8WithoutBom = new UTF8Encoding(false);
            File.WriteAllText(filename, sb.ToString(), utf8WithoutBom);
        }

        #endregion

        public static void clearConcurrentQueue(ConcurrentQueue<string[]> log)
        {
            int logCount = log.Count;
            for (int i = 0; i < logCount; i++)
            {
                log.TryDequeue(out string[] logLine);
            }
        }
    }
}
