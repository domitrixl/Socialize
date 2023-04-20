using APIHandler;
using DataExtensions;
using SocialClassCollection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTCP_IP
{
    internal class UserSeason
    {
        public string UserId;
        public bool Verified;
        public string userPubKey;
        public TcpClient Client;

        public NetworkStream stream;
        public static string ServerPubKey;
        public static string ServerPrivKey;

        //public UserEvents Events;

        public UserSeason(TcpClient client)
        {
            //if (ServerPubKey == null)
            //    throw new ArgumentNullException(nameof(ServerPubKey), "Please assign ServerPub before creating an instance of UserSeason!");
            Client = client;
            stream = Client.GetStream();

            //_ = SendVerifyConnection();
            _ = SendPublicKey();

            //Events = new UserEvents(Client);
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count) =>
            await stream.WriteAsync(buffer, offset, count);

        public void SetPubKeyUser(byte[] data)
        {
            userPubKey = Convert.ToBase64String(data).Base64Decode();
            Console.Write(userPubKey);
        }

        public async Task WriteAsync(ServerRequest serverRequest, byte[] data, bool encrypt)
        {
            //string encrytedText = Convert.ToBase64String(await Encryption.Encrypt(data, ServerPubKey));
            byte[] encrypted = encrypt ? await Encryption.Encrypt(data, userPubKey) : data;
            // mixed array of (index 0) UTF-8 and (index > 0) Base64 Array
            byte[] full = new byte[encrypted.Length + 1];
            full[0] = (byte)serverRequest;

            encrypted.CopyTo(full, 1);

            //List<byte> bytes = Encryption.Encrypt(data, ServerPubKey).ToList<byte>();
            //bytes.Insert(0, (byte)serverRequest);

            //string fullString = (byte)serverRequest + Convert.ToBase64String(encrypted).Base64Decode();
            //Console.WriteLine(fullString);
            //await ReadAsync(full);

            await WriteAsync(full, 0, full.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Base64 Array with decrypted Data</returns>
        public async Task<byte[]> ReadAsync(byte[] data)
        {
            //skip header
            data = data.Skip(1).ToArray();
            byte[] temp = await Encryption.Decrypt(data, ServerPrivKey);
            Console.WriteLine(Convert.ToBase64String(temp).Base64Decode());
            return temp;

            //string endResult = Convert.ToBase64String(temp);
            //string ready = endResult.Base64Decode();
            //Console.WriteLine(ready);
        }

        public async Task SendPublicKey() =>
            await WriteAsync(ServerRequest.ServerPubKey, Encoding.UTF8.GetBytes(ServerPubKey), false);

        public async Task SendVerifyConnection()
        {
            byte[] verify = { 0 };
            await WriteAsync(verify, 0, verify.Length);
        }

        public async Task<byte[]> RecieveData()
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

        public async Task<bool> VerifyUser(byte[] info)
        {
            // a char needs 1 byte
            string username = Encoding.UTF8.GetString(info.SubArray(0, 30));

            string token = Encoding.UTF8.GetString(info.SubArray(31, 50));

            return await Handler.VerifyToken(username, token);
        }

        public async Task<List<Message>> RequestUnreadMessages(byte[] info)
        {
            string id = Encoding.UTF8.GetString(info);

            return await Handler.RequestMessages(id);
        }

        public async Task HandleClientData(byte[] data)
        {
            if (data.Length != 0)
            {
                ClientRequest cr = (ClientRequest)data[0];
                byte[] info = data.Skip(1).ToArray();
                data = null;

                // first byte is the type of request
                switch (cr)
                {
                    case ClientRequest.UserPubKey:
                        SetPubKeyUser(info);
                        //TODO: set online status in DB
                        break;

                    case ClientRequest.VerifyUser:
                        await VerifyUser(info);
                        break;

                    case ClientRequest.GetUnreadMessages:
                        await RequestUnreadMessages(info);
                        //send to user
                        break;
                } 
            }
        }
    }
}