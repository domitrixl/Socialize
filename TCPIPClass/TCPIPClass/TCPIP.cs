using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPIPBasicFunctions
{
    public class TCPIP
    {
        public SslStream networkStream;

        public TCPIP(SslStream sslStream)
        {
            networkStream = sslStream;
        }

        public async Task SendAsync(byte[] message)
        {
            // Array of 4 bytes -> if one byte (max 255) is filled -> next byte +1
            // e.g. 12 -> first byte = 12, the others = 0
            // e.g. 256 -> first byte = 0, second byte = 1
            // e.g. 257 -> first byte = 1, second byte = 1
            // e.g. 512 -> first byte = 0, second byte = 2
            byte[] messageSize = BitConverter.GetBytes(message.Length);
            // first sends the size of the message, then the actual message
            
            await networkStream.WriteAsync(messageSize, 0, messageSize.Length).ConfigureAwait(false);
            await networkStream.WriteAsync(message, 0, message.Length).ConfigureAwait(false);
        }

        public async Task<byte[]> ReceiveAsync()
        {
            byte[] messageBuffer = null;
            // Byte-Array of Int32 is size of 4 default -> also fixed client-sided
            byte[] lengthBuffer = new byte[4];

            try
            {
                // gets the buffersize of the message - "first message"
                // assigns lengthbuffer the read bytes
                int i = await networkStream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length).ConfigureAwait(false);

                int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                messageBuffer = new byte[messageLength];

                // reads the actual message
                await ReadToEndAsync(networkStream, messageBuffer, messageLength).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // Error occured during receiving.
                throw;
            }
           
            return messageBuffer;
        }

        private async Task ReadToEndAsync(SslStream stream, byte[] messageBuffer, int messageLength)
        {
            int offset = 0;

            while (offset < messageLength)
            {
                // assigns message buffer the read bytes
                int bytesRead = await stream.ReadAsync(messageBuffer, offset, messageLength).ConfigureAwait(false);

                if (bytesRead == 0)
                {
                    // Socket is closed.
                    break;
                }

                // "marks" the bytes as read/assigned
                offset += bytesRead;
            }
        }
    }
}