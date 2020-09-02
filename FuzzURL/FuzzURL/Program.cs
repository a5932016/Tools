using System;
using System.IO;
using System.Net;

namespace FuzzURL
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string url = string.Empty;
                try
                {
                    url = args[0];
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Usage: {0} URL\r\n",
                        System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                }

                int index = url.IndexOf("?");
                string[] parms = url.Remove(0, index + 1).Split('&');
                foreach (string parm in parms)
                {
                    string xssUrl = url.Replace(parm, parm + "fd<xss>sa");
                    string sqlUrl = url.Replace(parm, parm + "fd'sa");

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sqlUrl);
                    request.Method = "GET";

                    string sqlresp = string.Empty;
                    using (StreamReader rdr = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        sqlresp = rdr.ReadToEnd();
                    }

                    request = (HttpWebRequest)WebRequest.Create(xssUrl);
                    request.Method = "GET";
                    string xssresp = string.Empty;

                    using (StreamReader rdr = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        xssresp = rdr.ReadToEnd();
                    }

                    if (xssresp.Contains("<xss>"))
                        Console.WriteLine("Possible XSS point found in patameter:" + parm);
                    else
                        Console.WriteLine("XSS does not seem vulnerable.");

                    if (sqlresp.Contains("error in your SQL syntax"))
                        Console.WriteLine("SQL injection point found in parameter:" + parm);
                    else
                        Console.WriteLine("SQL does not seem vulnerable.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
