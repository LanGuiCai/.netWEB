using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPSocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //建立TcpClient对象，并且连接到端口4001上的localhost(默认为127.0.0.1)
                TcpClient newclient = new TcpClient();
                newclient.Connect("localhost", 4001);
                NetworkStream stm = newclient.GetStream();

                //使用NetworkStream发送对象
                byte[] sendBytes = Encoding.ASCII.GetBytes("Data is commint" + "here");
                //将数据写入NetworkStream
                stm.Write(sendBytes, 0, sendBytes.Length);
                newclient.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
