using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
namespace wphNotificationService
{
    class Program
    {
        static TcpListener tcpListener;
        static Thread listenThread;

        static void Main(string[] args)
        {
            Console.WriteLine("WindowsPhoneHacker");
            Console.WriteLine("Notifications Service");

            tcpListener = new TcpListener(IPAddress.Any, 8081);
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
                Console.WriteLine("Something exciting");
                TcpClient mspush = new TcpClient("127.0.0.1", 443);

                NetworkStream clientStream = client.GetStream();
                NetworkStream mspushStream = mspush.GetStream();
                while (client.Client.Connected)
                {
                    if (clientStream.DataAvailable)
                    {
                        //StreamReader reader = new StreamReader(clientStream);
                        byte[] buf = new byte[1024];

                        int read = clientStream.Read(buf, 0, buf.Length);
                        Console.WriteLine(read);

                        mspushStream.Write(buf, 0, read);
                        string data = System.Text.ASCIIEncoding.ASCII.GetString(buf, 0, read);

                        Console.WriteLine(">>" + data);
                    }
                    if (mspushStream.DataAvailable)
                    {
                        byte[] buf = new byte[1024];

                        int read = mspushStream.Read(buf, 0, buf.Length);

                        clientStream.Write(buf, 0, read);

                        string data = System.Text.ASCIIEncoding.ASCII.GetString(buf, 0, read);

                        Console.WriteLine("<<" + data);

                        if (data.Contains("X-WindowsPhone-Target:toast"))
                        {
                            string s;
                            string notification;

                            s = data.Substring(data.IndexOf("<?xml"));
                            notification = s;

                            FileStream file = new FileStream(@"\Applications\Data\9d85578e-4908-43f3-a9e6-8ba5d297b817\Data\IsolatedStore\notification_" + DateTime.Now.ToString("MMddYYHHmmssfff") + ".txt", FileMode.Create, FileAccess.Write);
                            //FileStream file = new FileStream(@"notifications.txt", FileMode.OpenOrCreate, FileAccess.Write);
                            StreamWriter writer = new StreamWriter(file);
                            writer.Write(notification);
                            writer.Flush();
                            file.Close();

                        }
                    }
                    
                    System.Threading.Thread.Sleep(10);
                    
                }
                System.Threading.Thread.Sleep(100);

            }
        }
    }
}
