using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Socialize.Classes
{
    //https://stackoverflow.com/questions/18485715/how-to-use-public-and-private-key-encryption-technique-in-c-sharp
    internal class Encryption
    {
        public readonly string _privateKey;
        public readonly string _publicKey;
        private static readonly UnicodeEncoding _encoder = new UnicodeEncoding();

        public Encryption()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            _privateKey = rsa.ToXmlString(true);
            _publicKey = rsa.ToXmlString(false);
        }

        public static async Task<byte[]> Decrypt(byte[] encryptedData, string privateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.FromXmlString(privateKey);
            Task<byte[]> decrypt = Task.Run<byte[]>(() => 
            {
                return rsa.Decrypt(encryptedData, true); 
            });
            byte[] decrytedData = await decrypt;
            rsa.Dispose();
            return decrytedData;
        }

        public static async Task<byte[]> Encrypt(byte[] data, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            rsa.FromXmlString(publicKey);
            Task<byte[]> encrypt = Task.Run<byte[]>(() => {
                return rsa.Encrypt(data, true);
                }
            );
            byte[] encryptedData = await encrypt;
            rsa.Dispose();
            return encryptedData;
        }
    }
}
