using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPSocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //创建Tcplistener对象，侦听4001端口，使用Start方法
                IPAddress adr = IPAddress.Parse("127.0.0.1");
                TcpListener listener = new TcpListener(adr,4001);
                listener.Start();

                /*AcceptTcpClient()方法接受一个连续请求，返回一个TcpClient，使用它的
                 * GetStream方法来创建NetWorkStream对象
                 */
                TcpClient tc = listener.AcceptTcpClient();
                NetworkStream stm = tc.GetStream();//返回用于数据发送接收的NetworkStream

                byte[] readBuf = new byte[50];
                //使用Read方法读数据
                stm.Read(readBuf, 0, 50);
                Console.WriteLine(Encoding.ASCII.GetString(readBuf));
                stm.Close();//关闭连接
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
