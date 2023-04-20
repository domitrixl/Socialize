using Socialize.Classes;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Socialize
{
    public static class APIHandler
    {
        internal static MethodHandler methodHandler = new MethodHandler();

        static APIHandler()
        {
             
        } 
        public static void SetBearer(string bearer) =>
            methodHandler.SetBearer(bearer);

        public static void ClearBearer() =>
            methodHandler.ClearBearer();

        public static class Users
        {
            public static async Task<List<User>> SearchByUsername(string username) => 
                await methodHandler.Get<List<User>>(URI.User.SearchByUsername(username));

            public static async Task<User> GetByUserId(string userId) =>
               await methodHandler.Get<User>(URI.User.GetByUserId(userId));

            public static async Task<string> Register(string username, string password) =>
                 await(await methodHandler.Post(URI.User.Register(username, password), "", "")).Content.ReadAsStringAsync();

            public static async Task<string> Login(string username, string password)
            {
                try
                {
                    HttpResponseMessage response = await methodHandler.Post(URI.User.Login(username, password), "", "");

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        throw new ArgumentException("Error while login, check credentials");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            public static async Task<bool> AuthTest(string username, string password) =>
                (await methodHandler.Get(URI.User.Register(username, password))).IsSuccessStatusCode;

            public static async Task<bool> Delete(string userId) =>
               (await methodHandler.Delete(URI.User.Delete(userId))) == HttpStatusCode.OK;
        }

        /// <summary>
        /// mostly bearer needed config via APIHandler.SetBearer(string bearer) or clear via APIHandler.ClearBearer()
        /// </summary>
        public static class SocialManage
        {
            public static async Task<SocialManagement> GetByUserId(string userId) =>
                await methodHandler.Get<SocialManagement>(URI.SocialManagement.GetByUserId(userId));

            public static async Task<bool> SendFriendRequest(string userId) =>
              (await methodHandler.Put(URI.SocialManagement.SendFriendRequest(userId))).IsSuccessStatusCode;

            public static async Task<bool> AcceptFriendRequest(string userId) =>
             (await methodHandler.Put(URI.SocialManagement.AcceptFriendRequest(userId))).IsSuccessStatusCode;

            public static async Task<SocialManagement> Self() =>
            await methodHandler.Get<SocialManagement>(URI.SocialManagement.GetSelf);

            public static async Task<bool> Delete(string userId) =>
              (await methodHandler.Delete(URI.SocialManagement.DeleteFriendRequest(userId))) == HttpStatusCode.OK;
        }

        /// <summary>
        /// mostly bearer needed config via APIHandler.SetBearer(string bearer) or clear via APIHandler.ClearBearer()
        /// </summary>
        public static class Post
        {
            public static async Task<List<SocialPost>> GetByUserId(string userId) =>
                await methodHandler.Get<List<SocialPost>>(URI.SocialPost.GetByUserId(userId));

            public static async Task<List<SocialPost>> GetNewestFromFriends() =>
              (await methodHandler.Get<List<SocialPost>>(URI.SocialPost.GetNewerThan(DateTime.UtcNow)));

            public static async Task<List<SocialPost>> GetFromFriendsNewerThan(DateTime dateTime) =>
              (await methodHandler.Get<List<SocialPost>>(URI.SocialPost.GetNewerThan(DateTime.UtcNow)));

            public static async Task<List<SocialPost>> GetFromFriends(string userId) =>
             (await methodHandler.Get<List<SocialPost>>(URI.SocialPost.GetNewerThan(DateTime.UtcNow.AddYears(-100))));

            /// <summary>
            /// 
            /// </summary>
            /// <param name="socialPost">use the path you get from APIHandler.Post.Upload as socialPost.Media<br>Or simply use NewPost(SocialPost, byte[], string)</br></param>
            /// <returns></returns>
            public static async Task<bool> NewPost(SocialPost socialPost) =>
             (await methodHandler.Post<SocialPost>(URI.SocialPost.NewPost(), socialPost)).IsSuccessStatusCode;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="media"></param>
            /// <param name="contentType"></param>
            /// <returns>string: if successfully -> mediaPath, else null</br></returns>
            public static async Task<string> UploadMedia(byte[] media, string contentType)
            {
                HttpResponseMessage response = await methodHandler.Post(URI.SocialPost.UploadFile(), media, contentType);

                if (response.IsSuccessStatusCode)
                {
                    return response.Headers.Location.ToString();
                }
                return null;
            }

            public static async Task<bool> NewPost(SocialPost socialPost, byte[] media, string contentType)
            {
                string path = await UploadMedia(media, contentType);
                if (path is not null)
                {
                    socialPost.Media = path;
                    return await NewPost(socialPost);
                }
                return false;
            }

            public static async Task<byte[]> GetFile(string mediaPath)
            {
                HttpResponseMessage response = await methodHandler.Get(URI.SocialPost.GetFile(mediaPath));

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
            
            public static async Task<bool> Delete(string postId) =>
              (await methodHandler.Delete(URI.SocialPost.Delete(postId))) == HttpStatusCode.OK;
        }
        /// <summary>
        /// mostly bearer needed config via APIHandler.SetBearer(string bearer) or clear via APIHandler.ClearBearer()
        /// filter in front-end by message.DateTime < DateTime.Now or something sim.
        /// </summary>
        public static class Messaging
        {
            /// <summary>
            /// Encrypts a message with the pubKey of the given user (UserId)
            /// UserId (reciever) required!
            /// MessageText required!
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public static async Task<Message> EncryptMessage(Message message)
            {
                byte[] encrypted = await Encryption.Encrypt(Encoding.UTF8.GetBytes(message.Text), (await APIHandler.SocialManage.GetByUserId(message.ToUser)).pubKey);
                message.Text = Encoding.UTF8.GetString(encrypted);

                return message;
            }

            public static async Task<bool> PostMessage(Message message) =>
                (await methodHandler.Post<Message>(URI.Message.PostMessage, message)).IsSuccessStatusCode;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="userId"></param>
            /// <returns>Use Encryption.Decrypt to decrypt the utf8 message.text</returns>
            public static async Task<List<Message>> GetBySender(string userId) =>
              (await methodHandler.Get<List<Message>>(URI.Message.GetBySender(userId)));

            /// <summary>
            /// 
            /// </summary>
            /// <param name="userId"></param>
            /// <returns>Use Encryption.Decrypt to decrypt the utf8 message.text</returns>
            public static async Task<List<Message>> GetByReciever(string userId) =>
              (await methodHandler.Get<List<Message>>(URI.Message.GetBySender(userId)));
        }
    }
}