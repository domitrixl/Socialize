
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using Fithub.Classes;
using MongoDBAPI;
using System.Net.Mime;
using System.IO;
using System.Text;
using MongoDBAPI.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Services
{
    public class SocialPostService
    {
        private readonly IMongoCollection<SocialPost> _SocialPostCollection;

        public SocialPostService(
            IOptions<Database> DatabaseSettings)
        {
            var mongoClient = new MongoClient(
                DatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                DatabaseSettings.Value.DatabaseName);

            _SocialPostCollection = mongoDatabase.GetCollection<SocialPost>(
                DatabaseSettings.Value.CollectionNames[2]);
        }

        public async Task Delete(string id) =>
            await _SocialPostCollection.DeleteOneAsync(post => post.Id.Equals(id));

        public async Task<List<SocialPost>> GetAsync() =>
            await _SocialPostCollection.Find(_ => true).ToListAsync();

        public async Task<SocialPost> GetById(string id) =>
         await _SocialPostCollection.Find(post => post.Id.Equals(id)).FirstOrDefaultAsync();

        public async Task<List<SocialPost>> GetAllPostsFromUser(string id) =>
         await _SocialPostCollection.Find(post => post.PostedBy == id).ToListAsync();

        public async Task<List<SocialPost>> GetPostsOlderThan(List<string> friends, DateTime oldestPost)
        {
            return (await _SocialPostCollection.Find(post => post.DateTime > oldestPost && friends.Contains(post.PostedBy)).SortByDescending(post => post.DateTime).Limit(10).ToListAsync());
        }

        public async Task NewSocialPost(SocialPost socialPost)
            => await _SocialPostCollection.InsertOneAsync(socialPost);

        public async Task<ActionResult> UploadFile(HttpRequest httpContRequ, string userId)
        {
            string[] contenttype;
            if (httpContRequ.Headers.ContentType.Count >= 0)
                contenttype = httpContRequ.Headers.ContentType[0].Split('/');
            else
            {
                return new NoContentResult();
            }

            if (contenttype[0].ToLower().Equals("audio") || contenttype[0].ToLower().Equals("image") || (contenttype[0].ToLower().Equals("application") && contenttype[1].ToLower().Equals("json")))
            {
                string curPath = Path.Combine(Directory.GetCurrentDirectory(), "users", userId.ToLower(), contenttype[0]);
                Directory.CreateDirectory(curPath);

                httpContRequ.EnableBuffering();

                byte[] data = await ReadFully(httpContRequ.Body);
                if ((int)httpContRequ.Body.Length > 1024 * 1000000)
                {
                    return new StatusCodeResult(StatusCodes.Status413PayloadTooLarge);
                }

                int count = 0;
                string path = Path.Combine(curPath, DateTime.UtcNow.ToString("ddMMyyyy-HH-mm").Trim() + "." + contenttype[1]);
                while (File.Exists(path))
                {
                    count++;
                    path = Path.Combine(curPath, DateTime.UtcNow.ToString("ddMMyyyy-HH-mm").Trim() + "(" + count + ")." + contenttype[1]);
                }

                File.WriteAllBytes(path, data);
                return new CreatedResult(path, httpContRequ.Headers.ContentType[0]);
            }

            return new UnsupportedMediaTypeResult();
        }

        public static async Task<byte[]> ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await input.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public async Task<(string, byte[])> GetFile(string path)
        {
            //e.g. /API/users/{userId}/application/somedata.json
            if (File.Exists(path))
            {
                string[] splittedPath = path.Split('/');
                string contenttype = splittedPath[splittedPath.Length - 2];
                string[] filenameAndExt = splittedPath[splittedPath.Length - 1].Split('.');
                contenttype += "/" + filenameAndExt[1];
                return (contenttype, await File.ReadAllBytesAsync(path));
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        //public static async Task<string> GetRequestBody(HttpRequest request)
        //{
        //    var bodyStream = new StreamReader(request.Body);
        //    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
        //    var bodyText =  await bodyStream.ReadToEndAsync();
        //    return bodyText;
        //}

        //public async Task<string> GetImageFromUser(string username)
        //{

        //}
    }
}
