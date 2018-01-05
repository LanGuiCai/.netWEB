using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] data = new byte[1024];
            string input, stringData;

            IPEndPoint IPep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            try
            {
                server.Connect(IPep);
            }
            catch(Exception e)
            {
                Console.WriteLine("无法连接服务器"+e.ToString());
            }

            //接收 数据
            int receive = server.Receive(data);
            stringData = Encoding.ASCII.GetString(data, 0, receive);
            Console.WriteLine(stringData);

            while (true)
            {
                input = Console.ReadLine();
                if(input=="exit")
                {
                    break;
                }
                //发送数据
                server.Send(Encoding.ASCII.GetBytes(input));
                data = new byte[1024];
                receive = server.Receive(data);
                stringData = Encoding.ASCII.GetString(data,0,receive);
                Console.WriteLine(stringData);
            }
            //释放资源
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }
    }
}
