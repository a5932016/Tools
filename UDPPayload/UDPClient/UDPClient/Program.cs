using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace UDPClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string host = string.Empty;
                int lport = 0;

                try
                {
                    host = args[0];
                    lport = int.Parse(args[1]);
                }
                catch
                {
                    throw new Exception(string.Format("Usage: {0} HOST PORT\r\n",
                        System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
                }

                using (UdpClient listener = new UdpClient(lport))
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, lport);
                    string output;
                    byte[] bytes;

                    using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        IPAddress addr = IPAddress.Parse(host);
                        IPEndPoint addrEP = new IPEndPoint(addr, lport);

                        Console.WriteLine("Enter command to send, blank line to quit");
                        while (true)
                        {
                            string command = Console.ReadLine();

                            byte[] buff = Encoding.ASCII.GetBytes(command);
                            try
                            {
                                sock.SendTo(buff, addrEP);

                                if (string.IsNullOrEmpty(command))
                                {
                                    sock.Close();
                                    listener.Close();
                                    return;
                                }

                                if (string.IsNullOrWhiteSpace(command))
                                    continue;

                                bytes = listener.Receive(ref remoteEP);
                                output = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                                Console.WriteLine(output);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(" Exception {0}", ex.Message);
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

