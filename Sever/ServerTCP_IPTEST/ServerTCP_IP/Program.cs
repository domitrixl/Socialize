using SocialClassCollection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Drawing;
using Image = System.Drawing.Image;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Documents;
using DataExtensions;

namespace ServerTCP_IP
{
     class Program
    {
        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
        private static readonly Encryption encryption = new Encryption();

        private readonly Dictionary<int, UserSeason> seasons = new Dictionary<int, UserSeason>();


        public static void Main(string[] args)
        {
            //string enc = encryption.Encrypt("Hello WOrld");
            //Console.WriteLine(enc);
            //string dec = encryption.Decrypt(enc);
            //Console.WriteLine("Decrypted: " + dec);

            int count = 1;

            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 5000);
            UserSeason.ServerPubKey = encryption._publicKey;
            UserSeason.ServerPrivKey = encryption._privateKey;

            ServerSocket.Start();
            Console.WriteLine("Listening to: " + ServerSocket.LocalEndpoint);

            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                lock (_lock) list_clients.Add(count, client);
                Console.WriteLine("Someone connected!!");
                Thread t;
                //if (client.GetStream().Equals("127.0.0.1"))
                //{
                //    t = new Thread(Handle_APIUpdate);
                //}
                //else
                //{
                    t = new Thread(ClientConnect);
                //}
                
                t.Start(count);
                count++;
            }
        }

        public static async void APIConnect(TcpClient apiClient)
        {
            NetworkStream stream = apiClient.GetStream();

            await RecieveData(stream);

            while (true)
            {
                byte[] data = await RecieveData(stream);
            }
        }
      

        public static async void HandleAPIData()
        {
            
        }

        public static async void SendToAPI()
        {

        }


        public static void SendToUser()
        {

        }

        public static async void ClientConnect(object o)
        {
            int id = (int)o;
            TcpClient client;
            

            lock (_lock) client = list_clients[id];

            // Get Connection Content
            UserSeason userSeason = new UserSeason(client);
           

            //byte[] pubkey = Encoding.UTF8.GetBytes("0" + encryption._publicKey);
            //await userSeason.WriteAsync(pubkey, 0, pubkey.Length);

            // init
            try
            {
                do
                {
                    byte[] data = await userSeason.RecieveData();
                    //await userSeason.WriteAsync(ServerRequest.VerifyUser ,data);
                    //...
                    await userSeason.HandleClientData(data);
                } while (false);
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
            }
        }

        public static async void Handle_clients(object o)
        {
            int id = (int)o;
            TcpClient client;

            lock (_lock) client = list_clients[id];

            // Get Connection Content
            NetworkStream stream = client.GetStream();
            

            await RecieveData(stream);

            while (true)
            {
                
                string data = "";
                
                data = Encoding.ASCII.GetString(await RecieveData(stream));

                //Console.WriteLine("\n-------------------------------------------------------------------\n");
                
                Console.Write(data);

                data = String.Empty;

                //Message msg = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(data);
                //Bitmap bmp;
                //using (var ms = new MemoryStream(msg.Image))
                //{
                //    bmp = new Bitmap(ms);
                //}
                //bmp.Save(@"C:\Users\Hannes\Downloads\Worksfine.png");
                
                //broadcast(data);
                //Console.WriteLine(data);
            }

            lock (_lock) list_clients.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        public static async Task<byte[]> RecieveData(NetworkStream stream)
        {
            try
            {
                byte[] buffer = new byte[1028];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    do
                    {
                        read = await stream.ReadAsync(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, read);
                    } while (read > 0 && stream.DataAvailable);
                    return ms.ToArray();
                }
            }
            catch { return new byte[0]; }  
        }

        public static void Broadcast(string data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            lock (_lock)
            {
                foreach (TcpClient c in list_clients.Values)
                {
                    NetworkStream stream = c.GetStream();

                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}
