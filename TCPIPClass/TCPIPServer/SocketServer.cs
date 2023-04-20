using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TCPIPBasicFunctions;
//using APIHandler;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace TCPIPServer
{
    public static class SslTcpServer
    {
        private static X509Certificate2 serverCertificate = null;
        private static TcpListener serverSocket;
        private static Dictionary<TCPIP, string> awailableClients;

        public static void StartServer(string certificate)
        {
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
            serverCertificate = new X509Certificate2(certificate);
            Console.WriteLine(serverCertificate.Verify());
            // IPHostEntry ipHostEntry = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 8888);

            serverSocket = new TcpListener(ipEndPoint);
            serverSocket.Start();
            Console.WriteLine("Asynchonous server socket is listening at: " + ipEndPoint.Address.ToString());
            WaitForClients();
        }

        public static async Task SendAsync(byte[] message, TCPIP client) => await client.SendAsync(message);
        public static async Task<byte[]> ReceiveAsync(TCPIP client) => await client.ReceiveAsync();

        private static void WaitForClients()
        {
            serverSocket.BeginAcceptTcpClient(new System.AsyncCallback(OnClientConnected), null);
        }

        private static void OnClientConnected(IAsyncResult asyncResult)
        {
            try
            {
                TcpClient clientSocket = serverSocket.EndAcceptTcpClient(asyncResult);
                if (clientSocket != null)
                    Console.WriteLine("Received connection request from: " + clientSocket.Client.RemoteEndPoint.ToString());
                HandleClientRequest(clientSocket);
            }
            catch
            {
                throw;
            }
            Console.ReadKey();
            WaitForClients();
        }

        private static async void HandleClientRequest(TcpClient clientSocket)
        {
            SslStream sslStream = new SslStream(clientSocket.GetStream(), false);

            sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);

            // Display the properties and settings for the authenticated stream.
            DisplaySecurityLevel(sslStream);
            DisplaySecurityServices(sslStream);
            DisplayCertificateInformation(sslStream);
            DisplayStreamProperties(sslStream);

            sslStream.ReadTimeout = 5000;
            sslStream.WriteTimeout = 5000;


            //Write your code here to process the data
            //Console.WriteLine(clientSocket.Connected);
            TCPIP client = new TCPIP(sslStream);

            //string username = Encoding.UTF8.GetString(await client.ReceiveAsync());
            //string token = Encoding.UTF8.GetString(await client.ReceiveAsync());

            ////bool verified = await Handler.VerifyToken(username, token);
            //bool verified = true;

            //username = verified ? username : "guest";
            //awailableClients.Add(client, username);
            string username = "username";

            while (clientSocket.Connected)
            {
                byte[] messageBuffer = await client.ReceiveAsync();
                Console.WriteLine("\n" + username + ": " + System.Text.Encoding.UTF8.GetString(messageBuffer, 0, messageBuffer.Length));
            }
            Console.WriteLine(clientSocket.GetStream().ToString() + " disconnected!");
        }

        static void DisplaySecurityLevel(SslStream stream)
        {
            Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
            Console.WriteLine("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength);
            Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
            Console.WriteLine("Protocol: {0}", stream.SslProtocol);
        }
        static void DisplaySecurityServices(SslStream stream)
        {
            Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
            Console.WriteLine("IsSigned: {0}", stream.IsSigned);
            Console.WriteLine("Is Encrypted: {0}", stream.IsEncrypted);
        }
        static void DisplayStreamProperties(SslStream stream)
        {
            Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
            Console.WriteLine("Can timeout: {0}", stream.CanTimeout);
        }
        static void DisplayCertificateInformation(SslStream stream)
        {
            Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);

            X509Certificate localCertificate = stream.LocalCertificate;
            if (stream.LocalCertificate != null)
            {
                Console.WriteLine("Local cert was issued to {0} and is valid from {1} until {2}.",
                    localCertificate.Subject,
                    localCertificate.GetEffectiveDateString(),
                    localCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Local certificate is null.");
            }
            // Display the properties of the client's certificate.
            X509Certificate remoteCertificate = stream.RemoteCertificate;
            if (stream.RemoteCertificate != null)
            {
                Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                    remoteCertificate.Subject,
                    remoteCertificate.GetEffectiveDateString(),
                    remoteCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Remote certificate is null.");
            }
        }
    }
}
