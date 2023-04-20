using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDBAPI.Classes;

namespace MongoDBAPI.Services
{
    public class MessageService
    {
        private readonly IMongoCollection<Message> _messages;

        public MessageService(
        IOptions<Database> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _messages = mongoDatabase.GetCollection<Message>(
                bookStoreDatabaseSettings.Value.CollectionNames[3]);
        }

        public async Task<List<Message>> GetAsync() =>
            await _messages.Find(_ => true).ToListAsync();

        public async Task<Message?> GetAsync(string id) =>
            await _messages.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<Message>> GetAsyncFromUser(string fromUserId) =>
            await _messages.Find(x => x.Userid == fromUserId).ToListAsync();

        public async Task<List<Message>> GetAsyncToUser(string toUserId) =>
            await _messages.Find(x => x.Userid == toUserId).ToListAsync();

        public async Task CreateAsync(Message newUserData) =>
            await _messages.InsertOneAsync(newUserData);

        //public async Task UploadData(byte[] )
        //{

        //}

        public async Task UpdateAsync(string id, Message updatedUserData) =>
            await _messages.ReplaceOneAsync(x => x.Id == id, updatedUserData);

        public async Task RemoveAsync(string id) =>
            await _messages.DeleteOneAsync(x => x.Id == id);
    }
}
