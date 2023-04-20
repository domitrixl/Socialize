using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TCPIPBasicFunctions;

namespace TCPIPClient
{
    public class SocketClient
    {
        private static Hashtable certificateErrors = new Hashtable();

        TcpClient clientSocket = new TcpClient();
        TCPIP server = null;

        public SocketClient() { }

        public SocketClient(string ipAddress, int port, string username) => Connect(ipAddress, port, username);

        public void Connect(string ipAddress, int port, string username)
        {
            clientSocket.Connect(ipAddress, port);

            SslStream sslStream = new SslStream(
                clientSocket.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
                );

            try
            {
                sslStream.AuthenticateAsClient(ipAddress);
            }
            catch { }

            //networkStream = clientSocket.GetStream();
            server = new TCPIP(sslStream);
            
            //await server.SendAsync(Encoding.UTF8.GetBytes(username));
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public async Task SendAsync(byte[] message) => await server.SendAsync(message);
        public async Task<byte[]> ReceiveAsync() => await server.ReceiveAsync();


        //internal async Task SendAsync(byte[] message)
        //{
        //    // Array of 4 bytes -> if one byte (max 255) is filled -> next byte +1
        //    // e.g. 12 -> first byte = 12, the others = 0
        //    // e.g. 256 -> first byte = 0, second byte = 1
        //    // e.g. 257 -> first byte = 1, second byte = 1
        //    // e.g. 512 -> first byte = 0, second byte = 2

        //    byte[] messageSize = BitConverter.GetBytes(message.Length);

        //    // first sends the size of the message, then the actual message
        //    await networkStream.WriteAsync(messageSize, 0, messageSize.Length).ConfigureAwait(false);
        //    await networkStream.WriteAsync(message, 0, message.Length).ConfigureAwait(false);
        //}

        //public void Close()
        //{
        //    clientSocket.Close();
        //}
        //public async Task<byte[]> ReceiveAsync()
        //{
        //    byte[] messageBuffer = null;
        //    byte[] lengthBuffer = new byte[4];

        //    try
        //    {
        //        await networkStream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length).ConfigureAwait(false);
        //        int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

        //        messageBuffer = new byte[messageLength];
        //        await ReadToEndAsync(networkStream, messageBuffer, messageLength).ConfigureAwait(false);
        //    }
        //    catch (Exception)
        //    {
        //        // Error occured during receiving.
        //        throw;
        //    }

        //    return messageBuffer;
        //}

        //private async Task ReadToEndAsync(Stream stream, byte[] messageBuffer, int messageLength)
        //{
        //    int offset = 0;
        //    while (offset < messageLength)
        //    {
        //        int bytesRead = await stream.ReadAsync(messageBuffer, offset, messageLength).ConfigureAwait(false);
        //        if (bytesRead == 0)
        //        {
        //            // Socket is closed.
        //            break;
        //        }

        //        offset += bytesRead;
        //    }
        //}
    }
}
