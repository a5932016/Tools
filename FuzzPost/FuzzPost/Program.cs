using System;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FuzzPost
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string[] requestLines;

                try
                {
                    requestLines = File.ReadAllLines(args[0]);
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Usage: {0} FILENAME\n",
                        System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                }

                string[] parms = requestLines[requestLines.Length - 1].Split('&');
                string host = string.Empty;
                StringBuilder requestBuilder = new StringBuilder();

                foreach (string ln in requestLines)
                {
                    if (ln.StartsWith("Host:"))
                        host = ln.Split(' ')[1].Replace("\r", string.Empty);
                    requestBuilder.Append(ln + "\n");
                }

                string request = requestBuilder.ToString() + "\r\n";
                Console.WriteLine(request);

                IPEndPoint rhost = new IPEndPoint(IPAddress.Parse(host), 80);
                foreach (string parm in parms)
                {
                    using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        sock.Connect(rhost);

                        string val = parm.Split('=')[1];
                        string req = request.Replace("=" + val, "=" + val + "'");

                        byte[] reqBytes = Encoding.ASCII.GetBytes(req);
                        sock.Send(reqBytes);

                        byte[] buf = new byte[sock.ReceiveBufferSize];

                        sock.Receive(buf);
                        string response = Encoding.ASCII.GetString(buf);

                        if (response.Contains("error in your SQL syntax"))
                        {
                            Console.WriteLine("Parameter " + parm + " seems vulnerable to SQL injection with value: " + val + "'");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}