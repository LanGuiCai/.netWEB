using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CSclientThread
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket client;
            byte[] buf = new byte[1024];
            string input;
            IPAddress local = IPAddress.Parse("127.0.0.1");
            IPEndPoint iep = new IPEndPoint(local, 13000);
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                client.Connect(iep);
            }
            catch(SocketException)
            {
                Console.WriteLine("无法连接到服务器");
                return;
            }
            
            while(true)
            {
                input = Console.ReadLine();
                //输入exit断开与服务器的连接
                if(input=="exit")
                {
                    break;
                }
                //发送数据
                client.Send(Encoding.ASCII.GetBytes(input)); //普通消息发送,通过Send重载可以定义为固定长度消息发送

                //接收数据
                int rec = client.Receive(buf);
                Console.WriteLine(Encoding.ASCII.GetString(buf, 0, rec));
            }
            Console.WriteLine("断开服务器的连接");
            client.Close();
        }
    }
}
