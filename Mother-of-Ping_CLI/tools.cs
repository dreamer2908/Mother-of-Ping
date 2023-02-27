using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Security.Principal;
using System.Net;
using System.Web.Security;
using System.Web;
using System.Security.Cryptography;

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
            int numberOfElements = 2;
            string text = File.ReadAllText(textFile);
            return csvParser(text, hasHeader, numberOfElements);
        }

        public static List<string[]> csvParser(string text, bool hasHeader, int numberOfElements)
        {
            List<string[]> result = new List<string[]>();
            StringReader sr = new StringReader(text);

            using (TextFieldParser parser = new TextFieldParser(sr))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                if (hasHeader) // skip the header if available
                    parser.ReadLine();

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    string[] elements = sanitizeStringArray(fields, numberOfElements);

                    if (elements[0].Length > 0)
                        result.Add(elements);
                }
            }
            return result;
        }

        public static void writeCsv_ConcurrentQueue(ConcurrentQueue<string[]> log, string filename, bool overwrite)
        {
            string csvContents = convertConcurrentQueueToCsv(log);

            if (overwrite)
            {
                File.WriteAllText(filename, csvContents);
            }
            else
            {
                File.AppendAllText(filename, csvContents);
            }
        }

        public static int writeCsv_ConcurrentQueue(ConcurrentQueue<string[]> log, string filename, bool overwrite, int tries = 1, int retryDelay = 1000)
        {
            string csvContents = convertConcurrentQueueToCsv(log);

            int i = 0;
            bool good = true;

            // try to write until it's successful or the number of time exceeds <tries>
            // <tries> = 0 is also valid, and understandable for fags
            for (i = 0; i < tries; i++)
            {
                try
                {
                    if (overwrite)
                    {
                        File.WriteAllText(filename, csvContents);
                    }
                    else
                    {
                        File.AppendAllText(filename, csvContents);
                    }
                }
                catch (Exception ex) when (
                       ex is DirectoryNotFoundException
                    || ex is IOException
                    || ex is UnauthorizedAccessException
                    || ex is System.Security.SecurityException
                )
                {
                    System.Threading.Thread.Sleep(retryDelay);
                    good = false;
                    continue;
                }

                good = true;
                break;
            }

            return (good) ? 0 : i; // if failed to write, return how many times it tries
        }

        private static string convertConcurrentQueueToCsv(ConcurrentQueue<string[]> log)
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

            string csvContents = sb.ToString();
            return csvContents;
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

        public static string generateCsvReport(pingWork[] workForce)
        {
            ConcurrentQueue<string[]> contents = prepareReport(workForce);

            return convertConcurrentQueueToCsv(contents);
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
                "Max Consecutive Failed On", // 7
                "Max Consecutive Failed Duration", // 8
                "% Failed", // 9
                "Last Ping Status", // 10
                "Last Ping Time", // 11
                "Average Ping Time", // 12
                "Last Succeed On", // 13
                "Last Failed On", // 14
                "Last Failed Duration", // 15
                "Minimum Ping Time", // 16
                "Maximum Ping Time" // 17
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
                    formatDateTimeForGridView(work.maxConsecutiveDownTimestampEnd), // 7
                    formatTimeSpanForGridView(work.maxConsecutiveDownDuration), // 8
                    work.percentDown, // 9
                    pingWork.pingStatusToText[work.lastReply_result], // 10
                    formatPingTimeForGridView(work.lastReply_time, work.upCount), // 11
                    formatPingTimeForGridView(work.avgPingTime, work.upCount), // 12
                    formatDateTimeForGridView(work.lastUpTimestamp), // 13
                    formatDateTimeForGridView(work.lastDownTimestamp), // 14
                    formatTimeSpanForGridView(work.lastDownDuration), // 15
                    formatPingTimeForGridView(work.minPingTime, work.upCount), // 16
                    formatPingTimeForGridView(work.maxPingTime, work.upCount) // 17
                };
                contents.Enqueue(line);
            }

            return contents;
        }

        public static string formatDateTime(DateTime d)
        {
            return d.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string formatDateTimeForGridView(DateTime d)
        {
            // the default "no value" datetime in pingWork is Datetime.MinValue
            // so return empty for that
            if (d == DateTime.MinValue || d == DateTime.MaxValue)
            {
                return string.Empty;
            }
            else
            {
                return formatDateTime(d);
            }
        }

        public static string formatTimeSpan(TimeSpan s)
        {
            if (s.TotalDays >= 1)
            {
                return s.ToString(@"dd\.hh\:mm\:ss");
            }
            else if (s.TotalHours >= 1)
            {
                return s.ToString(@"hh\:mm\:ss");
            }
            else
            {
                return s.ToString(@"hh\:mm\:ss");
            }
        }

        public static string formatTimeSpanForGridView(TimeSpan s)
        {
            if (s == TimeSpan.MinValue || s == TimeSpan.MinValue)
            {
                return string.Empty;
            }
            else
            {
                return formatTimeSpan(s);
            }
        }

        public static string formatPingTimeForGridView(float t)
        {
            return (t < 0) ? string.Empty : string.Format("{0:0.#}", t);
        }

        public static string formatPingTimeForGridView(float t, int upCount)
        {
            return (t < 0 || upCount == 0) ? string.Empty : string.Format("{0:0.#}", t);
        }

        public static void generateHtmlReport(pingWork[] workForce, string filename)
        {
            ConcurrentQueue<string[]> contents = prepareReport(workForce);

            writeHtmlReport(contents, filename);
        }

        public static string generateHtmlReport(pingWork[] workForce)
        {
            ConcurrentQueue<string[]> contents = prepareReport(workForce);

            return prepareHtmlReport(contents);
        }

        private static void writeHtmlReport(ConcurrentQueue<string[]> contents, string filename)
        {
            string htmlContents = prepareHtmlReport(contents);
            Encoding utf8WithoutBom = new UTF8Encoding(false);
            File.WriteAllText(filename, htmlContents, utf8WithoutBom);
        }

        private static string prepareHtmlReport(ConcurrentQueue<string[]> contents)
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

            string htmlContents = sb.ToString();
            return htmlContents;
        }

        #endregion

        #region password
        private static string Protect(string text, string purpose)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            byte[] stream = Encoding.UTF8.GetBytes(text);
            byte[] encodedValue = MachineKey.Protect(stream, purpose);
            return HttpServerUtility.UrlTokenEncode(encodedValue);
        }

        private static string Unprotect(string text, string purpose)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            byte[] stream = HttpServerUtility.UrlTokenDecode(text);
            byte[] decodedValue = MachineKey.Unprotect(stream, purpose);
            return Encoding.UTF8.GetString(decodedValue);
        }

        public static string encryptPassword(string text)
        {
            string re = Protect(text, "907331bf-0052-464b-be89-19a517e79f6c");
            return (string.IsNullOrEmpty(re)) ? string.Empty : re;
        }

        public static string decryptPassword(string text)
        {
            try
            {
                string re = Unprotect(text, "907331bf-0052-464b-be89-19a517e79f6c");
                return (string.IsNullOrEmpty(re)) ? string.Empty : re;
            }
            catch (Exception)
            {
                return text;
            }
        }

        private static Aes BuildAesEncryptor(string key)
        {
            var aesEncryptor = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            aesEncryptor.Key = pdb.GetBytes(32);
            aesEncryptor.IV = pdb.GetBytes(16);
            return aesEncryptor;
        }

        public static string EncryptStringAes(string clearText, string key)
        {
            var aesEncryptor = BuildAesEncryptor(key);
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aesEncryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                }
                var encryptedText = Convert.ToBase64String(ms.ToArray());
                return encryptedText;
            }
        }

        public static string DecryptStringAes(string cipherText, string key)
        {
            var aesEncryptor = BuildAesEncryptor(key);
            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aesEncryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                }
                var clearText = Encoding.Unicode.GetString(ms.ToArray());
                return clearText;
            }
        }

        private static string AesKey = "907331bf-0052-464b-be89-19a517e79f6c";

        public static string encryptPasswordAes(string text)
        {
            string re = EncryptStringAes(text, AesKey);
            return (string.IsNullOrEmpty(re)) ? string.Empty : re;
        }

        public static string decryptPasswordAes(string text)
        {
            try
            {
                string re = DecryptStringAes(text, AesKey);
                return (string.IsNullOrEmpty(re)) ? string.Empty : re;
            }
            catch (Exception)
            {
                return text;
            }
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

        public static void trimConcurrentQueue(ConcurrentQueue<string[]> log, int sizeLimit)
        {
            int logCount = log.Count;
            for (int i = 0; i < (logCount - sizeLimit); i++)
            {
                log.TryDequeue(out string[] logLine);
            }
        }

        public static TimeSpan roundTimeSpanToSecond(TimeSpan tmspan)
        {
            return new TimeSpan(tmspan.Days, tmspan.Hours, tmspan.Minutes, tmspan.Seconds + (int)Math.Round(1.0 * tmspan.Milliseconds / 1000, MidpointRounding.AwayFromZero));
        }

        public static string getTodayString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static string getNowStringForFilename()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        }

        public static void executeAsAdmin(string fileName, string arg, bool hide, bool wait)
        {
            Process proc = new Process();

            proc.StartInfo.FileName = fileName;
            proc.StartInfo.Arguments = arg;

            // set it to run in elevated mode
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";

            if (hide)
            {
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            proc.Start();
            if (wait)
            {
                proc.WaitForExit();
            }
        }

        public static bool isAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static string readTextFromUrl(string url)
        {
            // WebClient is still convenient
            // Assume UTF8, but detect BOM - could also honor response charset I suppose
            using (var client = new WebClient())
            using (var stream = client.OpenRead(url))
            using (var textReader = new StreamReader(stream, Encoding.UTF8, true))
            {
                string re = string.Empty;
                try {
                    re = textReader.ReadToEnd();
                }
                catch {
                    // ignore all errors
                }
                return re;
            }
        }
    }
}
