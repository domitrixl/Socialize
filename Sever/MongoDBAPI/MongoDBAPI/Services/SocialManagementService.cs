using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Fithub.Classes;
using MongoDBAPI;
using MongoDBAPI.Classes;
using System.Runtime.Intrinsics.X86;

namespace API.Services
{
    public class SocialManagementService
    {
        private readonly IMongoCollection<SocialManagement> _SocialPostCollection;
        public SocialManagementService(
            IOptions<Database> DatabaseSettings)
        {
            var mongoClient = new MongoClient(
                DatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                DatabaseSettings.Value.DatabaseName);

            _SocialPostCollection = mongoDatabase.GetCollection<SocialManagement>(
                DatabaseSettings.Value.CollectionNames[1]);
        }

        public async Task<List<SocialManagement>> GetAsync() =>
            await _SocialPostCollection.Find(_ => true).ToListAsync();

        public async Task<SocialManagement> GetByUser(string userId) =>
            await _SocialPostCollection.Find(x => x.UserId.Equals(userId)).FirstOrDefaultAsync();

        public async Task<List<string>?> GetFriends(string userId) =>
            (await _SocialPostCollection.Find(x => x.UserId.Equals(userId)).FirstOrDefaultAsync()).Friends;

        public async Task<bool> AreFriends(string userIdA, string userIdB) =>
             (await _SocialPostCollection.Find(x => x.UserId.Equals(userIdA)).FirstOrDefaultAsync()).Friends.Contains(userIdB);

        public async Task<List<string>?> GetRequestsSent(string userId) =>
          (await _SocialPostCollection.Find(x => x.UserId.Equals(userId)).FirstOrDefaultAsync()).FRequestsSent;

        public async Task<List<string>?> GetRequestsRecieved(string userId) =>
          (await _SocialPostCollection.Find(x => x.UserId.Equals(userId)).FirstOrDefaultAsync()).FRequestsRecieved;

        public async Task<bool> SendFriendRequest(string userSent, string userRecieve)
        {
            SocialManagement smSent = await GetByUser(userSent);
            SocialManagement smRecieved = await GetByUser(userRecieve);

            if (smSent != null && smRecieved != null)
            {
                if (!smSent.Friends.Contains(smRecieved.UserId) && !smRecieved.FRequestsRecieved.Contains(smSent.UserId))
                {
                    smSent.FRequestsSent.Add(smRecieved.UserId);
                    smRecieved.FRequestsRecieved.Add(smSent.UserId);

                    await _SocialPostCollection.ReplaceOneAsync(x => x.Id.Equals(smSent.Id), smSent);
                    await _SocialPostCollection.ReplaceOneAsync(x => x.Id.Equals(smRecieved.Id), smRecieved);

                    return true;
                }
            }
            return false;
        }

        public async Task AcceptFriendRequest(User userAccept, User userSent)
        {
            SocialManagement smA = await GetByUser(userAccept.Id);
            SocialManagement smS = await GetByUser(userSent.Id);

            if (smA != null && smS != null)
            {
                if (smA.FRequestsRecieved.Contains(userSent.Id) && smS.FRequestsSent.Contains(userAccept.Id))
                {
                    smA.Friends.Add(userSent.Id);
                    smS.Friends.Add(userAccept.Id);

                    smA.FRequestsRecieved.Remove(userSent.Id);
                    smS.FRequestsSent.Remove(userAccept.Id);

                    await _SocialPostCollection.ReplaceOneAsync(x => x.Id.Equals(smA.Id), smA);
                    await _SocialPostCollection.ReplaceOneAsync(x => x.Id.Equals(smS.Id), smS);
                }
            }
        }

        public async Task<bool> DeleteFriendRequest(string userId, string deleteAssociationFrom)
        {
            SocialManagement smSelf = await GetByUser(userId);
            SocialManagement smAssociate = await GetByUser(deleteAssociationFrom);

            if (smSelf != null && smAssociate != null)
            {
                bool edited = false;
                if (smSelf.FRequestsRecieved.Contains(smAssociate.UserId))
                {
                    smSelf.FRequestsRecieved.Remove(smAssociate.UserId);
                    smAssociate.FRequestsSent.Remove(smSelf.UserId);
                    edited = true;
                }
                else if (smSelf.FRequestsSent.Contains(smAssociate.UserId))
                {
                    smSelf.FRequestsSent.Remove(smAssociate.UserId);
                    smAssociate.FRequestsRecieved.Remove(smSelf.UserId);
                    edited = true;
                }

                if (edited)
                {
                    await _SocialPostCollection.ReplaceOneAsync(x => x.Id.Equals(smSelf.Id), smSelf);
                    await _SocialPostCollection.ReplaceOneAsync(x => x.Id.Equals(smAssociate.Id), smAssociate);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> RemoveFromFriends(string userId, string deleteAssociationFrom)
        {
            SocialManagement smSelf = await GetByUser(userId);
            SocialManagement smAssociate = await GetByUser(deleteAssociationFrom);

            if (smSelf != null && smAssociate != null)
            {
                if (await AreFriends(smSelf.Id, smAssociate.Id))
                {
                    smSelf.Friends.Remove(smAssociate.UserId);
                    smAssociate.Friends.Remove(smSelf.UserId);

                    return true;
                }

                await _SocialPostCollection.ReplaceOneAsync(x => x.Id.Equals(smSelf.Id), smSelf);
                await _SocialPostCollection.ReplaceOneAsync(x => x.Id.Equals(smAssociate.Id), smAssociate);
            }
            return false;
        }
    }
}
