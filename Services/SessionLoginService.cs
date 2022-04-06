using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB_Session_Login.Models.Session;
using MongoDB_Session_Login.Models.SessionLogin;
using System.Net;
using System.Net.Sockets;

namespace MongoDB_Session_Login.Services
{
    public class SessionLoginService
    {
        private readonly IMongoCollection<SessionLogin> _sessionLoginCollection;
        public SessionLoginService()
        {

        }
        public SessionLoginService(IOptions<SessionLoginDatabaseSettings> sessionLoginDatabaseSettings)
        {
            var mongoClient = new MongoClient(sessionLoginDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(sessionLoginDatabaseSettings.Value.DatabaseName);
            _sessionLoginCollection = mongoDatabase.GetCollection<SessionLogin>(sessionLoginDatabaseSettings.Value.CollectionName);
        }

        public async Task<List<SessionLogin>> GetAllAsync() =>
            await _sessionLoginCollection.Find(_ => true).ToListAsync();
        
        public async Task<SessionLogin> GetAsync(string loginName) => 
            await _sessionLoginCollection.Find(s => s.LoginName == loginName.Trim().ToLower()).FirstOrDefaultAsync();

        public async Task CreateAsync(string loginName, string ipAddress)
        {
            SessionLogin sessionLogin = new SessionLogin
            {
                Id = ObjectId.GenerateNewId().ToString(),
                TokenSession = Guid.NewGuid().ToString(),
                LoginName = loginName,
                IPAddress = ipAddress
            };
            await _sessionLoginCollection.InsertOneAsync(sessionLogin);
        }

        public async Task<string> UpdateTokenAsync(string loginName)
        {
            SessionLogin sessionLogin = await _sessionLoginCollection.Find(s => s.LoginName == loginName).FirstOrDefaultAsync();
            sessionLogin.TokenSession = Guid.NewGuid().ToString();
            await _sessionLoginCollection.ReplaceOneAsync(s => s.LoginName == loginName, sessionLogin);

            return sessionLogin.TokenSession;
        }

        //code longpv2 nháp
        public async Task<string> UpdateSessionToken(string loginName,string sessionLoginName)
        {
            SessionLogin sessionLogin = await _sessionLoginCollection.Find(s => s.LoginName == loginName).FirstOrDefaultAsync();
            sessionLogin.TokenSession = sessionLoginName;
            await _sessionLoginCollection.ReplaceOneAsync(s => s.LoginName == loginName, sessionLogin);

            return sessionLogin.TokenSession;
        }

        public async Task DeleteAsync(string loginName) =>
            await _sessionLoginCollection.DeleteOneAsync(s => s.LoginName == loginName);

        public async Task DeleteAllAsync() =>
            await _sessionLoginCollection.DeleteManyAsync(_ => true);

        public async Task<List<SessionLogin>> GetSessionLoginFromIPAdressAsync(string IPAddress) =>
            await _sessionLoginCollection.Find(s => s.IPAddress == IPAddress).ToListAsync();

        public async Task<bool> IsSessionTokenValid(string loginName, string currentSessionToken)
        {
            SessionLogin sessionLogin = await _sessionLoginCollection.Find(s => s.LoginName == loginName).FirstOrDefaultAsync();
            if (sessionLogin != null)
            {
                if (currentSessionToken == sessionLogin.TokenSession)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
