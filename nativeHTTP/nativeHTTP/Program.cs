using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
namespace nativeHTTP
{
    class Program
    {
        static TcpListener tcpListener;
        static Thread listenThread;

        static void Main(string[] args)
        {
            Console.WriteLine("WindowsPhoneHacker");
            Console.WriteLine("Simple HTTP server");
            Console.WriteLine("(by your friend, Jaxbot)");

            tcpListener = new TcpListener(IPAddress.Any, 80);
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
            
        }

        static void ListenForClients()
        {
            tcpListener.Start();
            
            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = tcpListener.AcceptTcpClient();
                
                NetworkStream clientStream = client.GetStream();
                StreamReader reader = new StreamReader(clientStream);
                
                string request = reader.ReadLine();

                Console.WriteLine(request);

                StreamWriter writer = new StreamWriter(clientStream);

                string response = "Jaxbot from WindowsPhoneHacker";

                if (request.Contains("Bootstrap")) {
                    response = "Dip:tcps://127.0.0.1:8080/";
                }
                if (request.Contains("egg"))
                {
                    response = "<a href='http://en.wikipedia.org/wiki/Cherpumple'>Cherpumple</a>";
                }
                if (request.Contains("kill"))
                {
                    throw new Exception();
                }

                writer.WriteLine("HTTP/1.1 200 OK");
                writer.WriteLine("Date: " + DateTime.Now.ToUniversalTime().ToString("r"));
                writer.WriteLine("Server: Jaxbot. Can I start you with something to drink?");
                writer.WriteLine("Content-Length: " + response.Length);
                writer.WriteLine("Connection: close");
                writer.WriteLine("Content-Type: text/html");
                writer.WriteLine();
                writer.WriteLine(response);
                writer.Flush();
                writer.Close();
                System.Threading.Thread.Sleep(10);
                client.Close();

                System.Threading.Thread.Sleep(1000);
                
            }
        }
    }
}
