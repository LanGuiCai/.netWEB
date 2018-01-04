using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace CSserver
{
    /// <summary>
    /// 套接字服务器代码
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //再笨地创建套接字，使用bind()方法将套接字与本地IP和端口号绑定

            Socket client;
            Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            IPAddress adr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ep = new IPEndPoint(adr, 3000);

            //类似客户端的connet()方法
            ServerSocket.Bind(ep);
            //开始监听
            ServerSocket.Listen(3);

            //开始传输数据
            while(true)
            {
                if((client=ServerSocket.Accept())!=null)
                {
                    Console.WriteLine("连接上客户端，发送数据");
                    byte[] message = { 10, 12, 30, 45, 20, 1, 0 };//结束标记为0
                    //send()方法是返回发送的字节数
                    Console.WriteLine("总计将发送" + client.Send(message) + "个字节数据");
                    Console.WriteLine("发送完成");
                    //关闭连接
                    client.Close();
                    break;
                }
            }
        }
    }
}
