using BCrypt.Net;
using Fithub.Classes;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDBAPI.Classes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace MongoDBAPI.Services
{

    public class UserService
    {
        private readonly IMongoCollection<User> _userData;
        private readonly string _key;

        public UserService(
        IOptions<Database> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _userData = mongoDatabase.GetCollection<User>(
                bookStoreDatabaseSettings.Value.CollectionNames[0]);

            _key = bookStoreDatabaseSettings.Value.JWTKey;
        }

        public async Task<string> Register(string username, string password)
        {
            bool userExists = await _userData.Find(x => x.username.Equals(username)).FirstOrDefaultAsync() == null ? false : true ;

            if (!userExists)
            {
                string d = Path.Combine(Directory.GetCurrentDirectory(), username.ToLower());
               Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "users", username.ToLower()));

                Encryption enc = new Encryption();
                string salt = BC.GenerateSalt();
                User newUser = new User()
                {
                    username = username.ToLower(),
                    salt = salt,
                    password = BC.HashPassword(password, salt)
                };
                await _userData.InsertOneAsync(newUser);

                SocialManagement sm = new SocialManagement()
                {
                    UserId = newUser.Id,
                    Username = username.ToLower(),
                    Friends = new List<string>(),
                    FRequestsSent = new List<string>(),
                    FRequestsRecieved = new List<string>(),
                    ProfilePic = Path.Combine(Directory.GetCurrentDirectory(), "defaultProfile.png")
                };

                return enc._privateKey;
            }
            return string.Empty;
        }

        public async Task<string> Login(string username, string plainPassword)
        {
            var _user = await _userData.Find(x => x.username.Equals(username)).FirstOrDefaultAsync();
            if (_user is null) return null;
            string hashedPW = BC.HashPassword(plainPassword, _user.salt);
            if (!_user.password.Equals(hashedPW)) return null;

            var tokenhandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(_key);

            var tokenDescripto = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, _user.Id),
                    new Claim(ClaimTypes.Name, _user.username),
                    new Claim("Password", _user.password)
                }),

                Expires = DateTime.UtcNow.AddMinutes(30),

                SigningCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenhandler.CreateToken(tokenDescripto);

            return tokenhandler.WriteToken(token);
        }


        public async Task<List<User>> GetAsync() =>
        await _userData.Find(_ => true).ToListAsync();

        public async Task<User?> GetAsync(string id) =>
            await _userData.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<User>?> GetAsyncByUser(string username) =>
            await _userData.Find(x => x.username == username).ToListAsync();

        public async Task CreateAsync(User newUserData) =>
            await _userData.InsertOneAsync(newUserData);

        public async Task UpdateAsync(string id, User updatedUserData) =>
            await _userData.ReplaceOneAsync(x => x.Id == id, updatedUserData);

        public async Task RemoveAsync(string id) =>
            await _userData.DeleteOneAsync(x => x.Id == id);
    }

}
