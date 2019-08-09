using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;

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
    }
}
