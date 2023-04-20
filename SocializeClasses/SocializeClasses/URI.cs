using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socialize
{
    public static class URI
    {
        public static Uri ApiURI = new Uri("https://localhost:7079/api/");

        public static class User
        {
            public static string Base { get; } = "UserData";

            public static string SearchByUsername(string username) =>
              $"{Base}/GetUser/{username}";

            public static string GetByUserId(string userId) =>
              $"{Base}/UserData/{userId}";

            public static string AuthTest(string username, string password) =>
               $"{Base}/AuthTest";

            public static string Register(string username, string password) =>
               $"{Base}/Register?username={username}&plainPassword=" + System.Web.HttpUtility.UrlEncode(password);

            public static string Login(string username, string password) =>
              $"{Base}/Login?username={username}&plainPassword=" + System.Web.HttpUtility.UrlEncode(password);

            public static string Delete(string userId) =>
              $"{Base}/" + System.Web.HttpUtility.UrlEncode(userId);
        }
        public static class SocialManagement
        {
            private static string Base { get; } = "SocialManagement";

            public static string GetByUserId(string id) =>
                $"{Base}/{id}";
            public static string SendFriendRequest(string id) =>
                $"{Base}/Friends/SendRequest/{id}";
            public static string AcceptFriendRequest(string id) =>
                $"{Base}/Friends/AcceptRequest/{id}";
            public static string GetSelf { get; } = $"{Base}/Self";

            public static string DeleteFriendRequest(string id) =>
                $"{Base}/Friends/RemoveRequest/{id}";
        }
        public static class SocialPost
        {
            private static string Base { get; } = "SocialPost";

            public static string GetByUserId(string id) =>
                $"{Base}/GetByUser?id={id}";

            public static string GetNewerThan(DateTime dateTime) =>
               System.Web.HttpUtility.UrlEncode($"{Base}/newest/{dateTime.ToLongDateString()}");

            public static string NewPost() =>
               $"{Base}/NewPost";

            public static string UploadFile() =>
                $"{Base}/UploadFile";
            public static string GetFile(string path) =>
                $"{Base}/GetFile/{path}";

            public static string Delete(string id) =>
               $"{Base}/Delete?id={id}";
        }
        public static class Message
        {
            private static string Base { get; } = "Message";

            public static string PostMessage { get; } = Base;
            public static string GetBySender(string messageId) =>
                $"{Base}/GetSender/{messageId}";

            public static string GetByReciever(string messageId) =>
                $"{Base}/GetReciever/{messageId}";
        }
    }
}
