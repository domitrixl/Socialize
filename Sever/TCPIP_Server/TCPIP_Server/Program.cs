using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TCPIPServer;

namespace TCPIP_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SocketServer.StartServer();
            Console.Read();
        }
    }
}
