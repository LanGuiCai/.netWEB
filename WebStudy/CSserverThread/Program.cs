using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace CSserverThread
{
    class Program
    {
        static void Main(string[] args)
        {
            Threadtcpserver instance = new Threadtcpserver();
        }
    }
    class Threadtcpserver
    {
        private Socket server;
        public Threadtcpserver()
        {
            //初始化IP
            IPAddress local = IPAddress.Parse("127.0.0.1");
            IPEndPoint iep = new IPEndPoint(local, 13000);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //将套接字不本地终结点绑定
            server.Bind(iep);
            //在本地13000端口号监听
            server.Listen(20);
            Console.WriteLine("等待客户端连接");

            while(true)
            {
                //得到包含客户端信息的套接字,如果没有连接则堵塞
                Socket client = server.Accept();
                //创建消息服务线程对象
                ClientThread newclient = new ClientThread(client);
                Thread newthread = new Thread(new ThreadStart(newclient.ClientService));
                newthread.Start();
            }
        }
    }

    class ClientThread
    {
        //连接数
        public static int connections = 0;
        public Socket service;
        int i;
        public ClientThread(Socket clientsocket)
        {
            this.service = clientsocket;
        }
        public void ClientService()
        {
            String data = null;
            byte[] bytes = new byte[1024];
            //如果Socket不是空，则连接数加一
            if(service!=null)
            {
                connections++;
            }
            Console.WriteLine("新客户端建立连接：{0}个连接数", connections);

            while((i=service.Receive(bytes))!=0)
            {
                //解码字符
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine("收到数据：{0}", data);
                //处理客户端发来的信息
                data = data.ToUpper();

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                //发送应答消息
                service.Send(msg);
                Console.WriteLine("发送数据：{0}", data);
            }
            //关闭套接字
            service.Close();
            connections--;
            Console.WriteLine("客户端关闭连接：{0}个连接数", connections);
        }
    }
}
