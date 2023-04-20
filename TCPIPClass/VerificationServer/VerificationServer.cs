using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using APIHandler;
using APIUrls;

namespace VerificationManager
{
    public static class VerificationServer
    {
        public static async Task<bool> UsernameVerifaction(string username, string password)
        {
            return await Verifacte(URLs.GetByUsername(username), password);
        }

        public static async Task<bool> EmailVerification(string email, string password)
        {
            return await Verifacte(URLs.GetByEmail(email), password);
        }

        public static async Task<bool> Verifacte(string requestPath, string password)
        {
            User temp = await Handler.GetData<User>(requestPath);

            if (temp != null && temp != new User())
            {
                if (PasswordEquals(password, temp.Password, temp.Salt))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a User and returns its private Key
        /// </summary>
        /// <param name="user"></param>
        /// <returns>private Key of the user created</returns>
        /// <exception cref="ArgumentException">User could not be created</exception>
        public static async Task<string> CreateUser(User user)
        {
            Tuple<string, string> tuple = CreateKey();
            user.PublicKey = tuple.Item1;

            Random rnd = new Random();
            user.Salt = CreateSalt(rnd.Next(64, 128));

            if (await Handler.InsertObject<User>(user, URLs.CreateUser))
                return tuple.Item2;

            throw new ArgumentException("Could not Create User!");
        }

        /// <summary>
        /// Creates a matching Public and Private Key
        /// </summary>
        /// <returns>item 1 = public key<br></br>
        /// item 2 = private key</returns>
        public static Tuple<string,string> CreateKey()
        {
            string privateKey, publicKey;

            var rsa = new RSACryptoServiceProvider();
            privateKey = rsa.ToXmlString(true);
            publicKey = rsa.ToXmlString(false);

            return Tuple.Create(publicKey, privateKey);
        }

        /// <summary>
        /// Checks if the hashed password and plain password are equal
        /// </summary>
        /// <param name="plainPW"></param>
        /// <param name="hashedPW"></param>
        /// <param name="salt"></param>
        /// <returns><b>true</b> = passwords are equal<br></br><b>false</b> = passwords are not equal</returns>
        private static bool PasswordEquals(string plainPW, string hashedPW, string salt)
        {
            string newHashed = CreateHash(plainPW, salt);
            return newHashed.Equals(hashedPW);
        }

        private static string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        private static string CreateHash(string input, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);

            SHA256Managed sha265string = new SHA256Managed();
            byte[] hash = sha265string.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

     
        //private static string Encrypt(string data, string publicKey)
        //{
        //    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        //    rsa.FromXmlString(publicKey);

        //    byte[] dataToEncrypt = Encoding.ASCII.GetBytes(data);
        //    byte[] encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();

        //    return Convert.ToBase64String(encryptedByteArray);
        //}

        //private static string Decrypt(string data, string privateKey)
        //{
        //    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        //    rsa.FromXmlString(privateKey);

        //    byte[] dataByte = Convert.FromBase64String(data);
        //    byte[] decryptedByte = rsa.Decrypt(dataByte, false);

        //    return Encoding.UTF8.GetString(decryptedByte);
        //}
    }
}
