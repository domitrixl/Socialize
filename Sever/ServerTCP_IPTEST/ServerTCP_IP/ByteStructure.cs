using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTCP_IP
{
    internal static class ByteStructure
    {
        public static readonly Dictionary<ClientRequest, byte> clientRequests = new Dictionary<ClientRequest, byte>
        {
            { ClientRequest.VerifyUser, 0 },
            { ClientRequest.GetUnreadMessages, 0 }
        };
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
