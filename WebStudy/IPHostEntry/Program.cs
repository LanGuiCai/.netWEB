using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace IPHostEntrySample
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length!=1)
            {
                Console.WriteLine("Usage:GetDNSHostInfo hostname");
                return;
            }

            IPHostEntry result = Dns.GetHostByName(args[0]);
            Console.WriteLine("Hostname：{0}", result.HostName);
            //循环输出ip地址表
            foreach(string alias in result.Aliases)
            {
                Console.WriteLine("Alias:{0}", alias);
            }
            //遍历地址列表
            foreach(IPAddress address in result.AddressList)
            {
                Console.WriteLine("Address:{0}", address.ToString());
            }
        }
    }
}
