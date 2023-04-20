using APIHandler;
using SocialClassCollection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Image = System.Drawing.Image;
using DataExtensions;
using E2EEncryption;
using System.Linq;

namespace ClientConsoleTest
{
    internal static class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string logedInAs = "guest";
        private static Encryption encryption = new Encryption();
        private static string serverPubKey;

        public static void Main()
        {
            //Login().GetAwaiter().GetResult();
            InitConnect(false);

            Console.ReadKey();
        }

        private static async Task Login()
        {
            //IFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(ns, msg);

            //Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(msg);

            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            //List<UserData> ud = await Handler.GetData<List<UserData>>("https://localhost:7079/api/UserData/GetUser/" + username);

            if (await Handler.DataExists("https://localhost:7079/api/UserData/GetUser/" + username))
            {
                UserData user = await Handler.GetData<UserData>("https://localhost:7079/api/UserData/GetUser/" + username);

                Console.WriteLine(user.username + ";" + user.password);

                if (username.Equals(user.username) && password.Equals(user.password))
                {
                    logedInAs = username;
                    Console.WriteLine("\nLogged in as " + logedInAs + "!");
                }
                else
                {
                    Console.WriteLine("Login failed!");
                }
            }
            else
            {
                Console.WriteLine("Please register!");
            }
        }

        private static Message msg = new Message
        {
            //Message message = new Message();
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Text = "TestText",
            Datatype = Datatype.Text,
            Read = false,
            ToUser = "reciever",
            UserId = "sender",
            Image = new byte[0],
        };

        private static async void InitConnect(bool loop)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 5000;
            TcpClient client = new TcpClient();

            
            client.Connect(ip, port);
            Console.WriteLine("client connected!!");


            NetworkStream ns = client.GetStream();

            Thread thread = new Thread(async o =>
            {
                byte[] data = await RecieveData((TcpClient)o);

                if (data != null)
                {
                    HandleData(data); 
                }
            });

            bool condition = loop;

            thread.Start(client);
            do
            {
                await ns.WriteToServer(ClientRequest.UserPubKey, Convert.FromBase64String(encryption._publicKey.Base64Encode()), false);
                //SendMessage(ns, msg);
                
            } while (condition);

            client.Client.Shutdown(SocketShutdown.Send);
            thread.Abort();
            thread.Join();
            ns.Close();
        }

        public static void SetPubKeyServer(byte[] data)
        {
            serverPubKey = Convert.ToBase64String(data).Base64Decode();
            Console.Write(serverPubKey);
        }

        public static void HandleData(byte[] data)
        {
            if (data.Length != 0)
            {
                ServerRequest cr = (ServerRequest)data[0];
                byte[] info = data.Skip(1).ToArray();
                data = null;

                // first byte is the type of request
                switch (cr)
                {
                    case ServerRequest.ServerPubKey:
                        SetPubKeyServer(info);
                        //TODO: set online status in DB
                        break;

                        //case ClientRequest.VerifyUser:
                        //    await VerifyUser(info);
                        //    break;

                        //case ClientRequest.GetUnreadMessages:
                        //    await RequestUnreadMessages(info);
                        //    //send to user
                        //    break;
                } 
            }
        }

        public static async Task WriteToServer(this NetworkStream ns, ClientRequest serverRequest, byte[] data, bool encrypt)
        {
            //string encrytedText = Convert.ToBase64String(await Encryption.Encrypt(data, ServerPubKey));

            byte[] encrypted = encrypt ? await encryption.Encrypt(data) : data;
            // mixed array of (index 0) UTF-8 and (index > 0) Base64 Array
            byte[] full = new byte[encrypted.Length + 1];
            full[0] = (byte)serverRequest;

            encrypted.CopyTo(full, 1);

            await ns.WriteAsync(full, 0, full.Length);
        }

        private static void SendMessage(NetworkStream ns, Message message)
        {
            //string content = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            string content = "Hello world!";
            Console.WriteLine(content);

            byte[] buffer = Convert.FromBase64String(content.Base64Encode());
            ns.Write(buffer, 0, buffer.Length);
        }


        private static void Chat()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");

            //Image image = Image.FromFile(@"C:\Users\Hannes\Downloads\Unbenannt.png");

            //byte[] imagebyte = ImageToByte(image);

            //imagebyte = ResizeToMax50Kbytes(imagebyte, 5000);

            int port = 5000;
            TcpClient client = new TcpClient();

            client.Connect(ip, port);
            Console.WriteLine("client connected!!");
            NetworkStream ns = client.GetStream();
            //Thread thread = new Thread(async () =>
            //{
            //    await RecieveData(ns);
            //});


            //thread.Start(client);

            string s;
            while (!string.IsNullOrEmpty((s = Console.ReadLine())))
            {
                Message msg = new Message(s, null, logedInAs, "ToTestUser", false);
                //IFormatter formatter = new BinaryFormatter();
                //formatter.Serialize(ns, msg);

                //Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(msg);
                string content = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                Console.WriteLine(content);

                //512000

                byte[] buffer = Encoding.ASCII.GetBytes(content);
                ns.Write(buffer, 0, buffer.Length);
            }

            client.Client.Shutdown(SocketShutdown.Send);
            //thread.Join();
            ns.Close();
            client.Close();
            Console.WriteLine("disconnect from server!!");
            Console.ReadKey();
        }

        public static async Task<byte[]> RecieveData(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
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
            catch { Console.WriteLine("Client disconnected"); }
            return null;
        }

        private static async Task<List<UserData>> GetProductAsync(string path)
        {
            //api String
            //example for username lugghons -> https://localhost:7079/api/UserData/GetUser/lugghons
            path = "https://localhost:7079/api/UserData";
            List<UserData> userList = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var o = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(o);
                //Console.ReadKey();
                userList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserData>>(o);

                //List<UserData> ud = await GetProductAsync(getData);
                //Console.Write(userList[0].username);
            }
            return userList;
        }

        public static byte[] ImageToByte(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public static byte[] ResizeToMax50Kbytes(byte[] byteImageIn, int byteSize)
        {
            byte[] currentByteImageArray = byteImageIn;
            double scale = 1f;

            MemoryStream inputMemoryStream = new MemoryStream(byteImageIn);
            Image fullsizeImage = Image.FromStream(inputMemoryStream);

            while (currentByteImageArray.Length > byteSize)
            {
                Bitmap fullSizeBitmap = new Bitmap(fullsizeImage, new Size((int)(fullsizeImage.Width * scale), (int)(fullsizeImage.Height * scale)));
                MemoryStream resultStream = new MemoryStream();

                fullSizeBitmap.Save(resultStream, fullsizeImage.RawFormat);

                currentByteImageArray = resultStream.ToArray();
                resultStream.Dispose();
                resultStream.Close();

                scale -= 0.05f;
            }

            return currentByteImageArray;
        }


    }
    public enum ClientRequest : byte
    {
        UserPubKey,
        VerifyUser,
        GetUnreadMessages,
    }

    public enum ServerRequest : byte
    {
        ServerPubKey,
        VerifyUser,
        GetUnreadMessages,
    }
}