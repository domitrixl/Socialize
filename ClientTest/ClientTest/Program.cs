using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TCPIPClient;

namespace ClientTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SocketClient client = new SocketClient("127.0.0.1", 8888);

            #region convertExample
            //string convert = "This is the string to be converted";
            //Console.WriteLine(convert);
            // From string to byte array
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(convert);
            //foreach (byte b in buffer)
            //{
            //    Console.Write(b);
            //}
            // From byte array to string
            //string s = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            //Console.WriteLine("/n", s);
            #endregion

            MainAsync(client).GetAwaiter().GetResult();
            Console.Read();
        }
        public static async Task MainAsync(SocketClient client)
        {
            await client.SendAsync(Encoding.ASCII.GetBytes("675t9786t9"));
        }
    }
}
