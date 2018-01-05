using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPServer
{
    /// <summary>
    /// 使用tcp/ip协议
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //就收到的数据
            int receive;
            byte[] data = new byte[1024];
            //侦听所有接口ip
            IPEndPoint IPep = new IPEndPoint(IPAddress.Any, 9050);
            //创建TCP套接字，与IPEndPoin对象进行绑定，收听接入连接
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            newsock.Bind(IPep);
            newsock.Listen(10);
            Console.WriteLine("等待数据");

            //创建一个新的连接
            Socket client = newsock.Accept();
            IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("连接在{0},端口{1}", clientep.Address, clientep.Port);

            string welcome = "welcome";
            data = Encoding.ASCII.GetBytes(welcome);
            client.Send(data, data.Length, SocketFlags.None);//发送数据

            while(true)
            {
                data = new byte[1024];
                receive = client.Receive(data);
                if(receive==0)
                {
                    break;
                }
                Console.WriteLine(Encoding.ASCII.GetString(data, 0, receive));
                client.Send(data, receive, SocketFlags.None);
            }
            Console.WriteLine("{0}断开连接", clientep.Address);
            client.Close();
            newsock.Close();
        }
    }
}
