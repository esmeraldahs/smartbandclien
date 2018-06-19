using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SmartBandClient
{
    class SocketClient
    {
        public static void StartClient()
        {
            var bytes = new byte[1024];

            while (true)
            {
                try
                {
                    var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    var ipAddress = ipHostInfo.AddressList[0];
                    var remoteEp = new IPEndPoint(ipAddress, 8000);

                    var sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    try
                    {
                        sender.Connect(remoteEp);
                        Console.WriteLine("Sending data to the server.");

                        var randomNr = new Random();
                        var heartBeat = randomNr.Next(50, 120);

                        var sendheartbeat = Encoding.ASCII.GetBytes(heartBeat.ToString());

                        sender.Send(sendheartbeat);
                        Console.WriteLine("Heart beat per minute = {0} is sent to server.", heartBeat);

                        var bytesRec = sender.Receive(bytes);
                        Console.WriteLine("Server response = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();
                        const int milliseconds = 5000;
                        Thread.Sleep(milliseconds);
                    }

                    catch (ArgumentNullException nullException)
                    {
                        Console.WriteLine("Socket disconnected because : {0}", nullException.Message);
                    }
                    catch (SocketException socketException)
                    {
                        Console.WriteLine("Socket disconnected because : {0}", socketException.Message);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Socket disconnected because : {0}", exception.Message);
                    }
                }

                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }

        public static int Main(string[] args)
        {
            StartClient();
            return 0;
        }
    }
}
