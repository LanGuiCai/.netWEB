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

        }
    }
}
