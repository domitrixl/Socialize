using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using APIHandler;
using SocialClassCollection;
using DataExtensions;

namespace ServerTCP_IP
{
    internal class UserEvents
    {
        private TcpClient _client;
        //private bool _authentificated;
        

        public UserEvents(TcpClient client)
        {
            _client = client;
        }

        public async Task<bool> VerifyUser(byte[] info)
        {
            // a char needs 1 byte
            string username = Encoding.UTF8.GetString(info.SubArray(0,30));

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
            byte request = data[0];
            byte[] info = data.Skip(1).ToArray();
            data = null;

            // first byte is the type of request
            switch (request)
            {
                case 0:
                    await VerifyUser(info);
                    break;
                case 1:
                    //TODO: set online status in DB
                    break;
                case 2:
                    await RequestUnreadMessages(info);
                    //send to user
                    break;

            }
        }
    }
}
