using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CSclient
{
    /// <summary>
    /// 套接字客户端代码
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //创建socket对象
            Socket Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //绑定IP地址
            IPAddress adr = IPAddress.Parse("127.0.0.1");
            //绑定端口
            IPEndPoint ep = new IPEndPoint(adr, 3000);
            //完成绑定
            Server.Connect(ep);

            int end = 0;
            byte[] buffer=new byte[10];

            //进行数据传输
            if(Server.Receive(buffer) >0)
            {
                Console.WriteLine("连接上....");
                Console.WriteLine("从服务器接收数据");

                while(buffer[end]!=0)
                {
                    //显示缓冲区数据
                    Console.WriteLine(buffer[end]);

                    end++;
                }

                Console.WriteLine("断开连接");

                //释放连接
                Server.Shutdown(SocketShutdown.Both);
                Server.Close();
            }
        }
    }
}
