using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB_Session_Login.Models;
using MongoDB_Session_Login.Models.Session;
using MongoDB_Session_Login.Models.SessionLogin;
using System.Net;
using System.Net.Sockets;

namespace MongoDB_Session_Login.Services
{
    public class SessionLoginService
    {
        private readonly IMongoCollection<SessionLogin> _sessionLoginCollection;
        private readonly IHttpContextAccessor _accessor;

        public SessionLoginService(IOptions<SessionLoginDatabaseSettings> sessionLoginDatabaseSettings, IHttpContextAccessor accessor)
        {
            var mongoClient = new MongoClient(sessionLoginDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(sessionLoginDatabaseSettings.Value.DatabaseName);
            _sessionLoginCollection = mongoDatabase.GetCollection<SessionLogin>(sessionLoginDatabaseSettings.Value.CollectionName);
            _accessor = accessor;
        }

        public async Task<List<SessionLogin>> GetAllAsync() =>
            await _sessionLoginCollection.Find(_ => true).ToListAsync();
        
        public async Task<SessionLogin> GetAsync(string loginName) => 
            await _sessionLoginCollection.Find(s => s.ALoginName == loginName.Trim()).FirstOrDefaultAsync();

        public async Task CreateAsync(string loginName)
        {
            UserAgent userAgent = GetInfoUserAgent();
            SessionLogin sessionLogin = new SessionLogin
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ALoginName = loginName,
                AToken = Guid.NewGuid().ToString(),
                AIpServer = GetLocalIPAddress(),
                AIpClient = _accessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                AUserAgent = userAgent.UserAgentString,
                ABrowser = userAgent.Browser,
                ALoginTime = DateTime.Now.ToString("dd-MMM-yy"),
                ALogoutTime = null,
                ASessionNo = loginName+DateTime.Now.ToString("yyyyMMddHHmmssffffff"),
                ASessionFirstLogin = 0,
                AIsMobile = "N",
                ABrowserName = userAgent.BrowserName,
                ABrowserVers = userAgent.BrowserVersion,
                ACHKPass2 = 0
            };

            await _sessionLoginCollection.InsertOneAsync(sessionLogin);
        }

        public async Task<SessionLogin> UpdateWhenLoginAsync(string loginName)
        {
            SessionLogin updateSessionLogin = await _sessionLoginCollection.Find(s => s.ALoginName == loginName).FirstOrDefaultAsync();

            UserAgent userAgent = GetInfoUserAgent();
            updateSessionLogin.AToken = Guid.NewGuid().ToString();
            updateSessionLogin.AIpServer = GetLocalIPAddress();
            updateSessionLogin.AIpClient = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            updateSessionLogin.AUserAgent = userAgent.UserAgentString;
            updateSessionLogin.ABrowser = userAgent.Browser;
            updateSessionLogin.ALoginTime = DateTime.Now.ToString("dd-MMM-yy");
            updateSessionLogin.ALogoutTime = null;
            updateSessionLogin.ASessionNo = loginName + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            updateSessionLogin.ABrowserName = userAgent.BrowserName;
            updateSessionLogin.ABrowserVers = userAgent.BrowserVersion;

            await _sessionLoginCollection.ReplaceOneAsync(s => s.ALoginName == loginName, updateSessionLogin);
            SessionLogin newSessionLogin = await _sessionLoginCollection.Find(s => s.ALoginName == loginName).FirstOrDefaultAsync();

            return newSessionLogin;
        }
        
        public async Task<SessionLogin> UpdateLogoutTimeAsync(string loginName)
        {
            SessionLogin updateSessionLogin = await _sessionLoginCollection.Find(s => s.ALoginName == loginName).FirstOrDefaultAsync();
            updateSessionLogin.ALogoutTime = DateTime.Now.ToString("dd-MMM-yy");
            await _sessionLoginCollection.ReplaceOneAsync(s => s.ALoginName == loginName, updateSessionLogin);
            SessionLogin newSessionLogin = await _sessionLoginCollection.Find(s => s.ALoginName == loginName).FirstOrDefaultAsync();

            return newSessionLogin;
        }

        public async Task DeleteAsync(string loginName) =>
            await _sessionLoginCollection.DeleteOneAsync(s => s.ALoginName == loginName);

        public async Task DeleteAllAsync() =>
            await _sessionLoginCollection.DeleteManyAsync(_ => true);

        public async Task<List<SessionLogin>> GetSessionLoginFromIPAdressAsync(string ipClient) =>
            await _sessionLoginCollection.Find(s => s.AIpClient == ipClient).ToListAsync();

        public async Task<bool> IsSessionTokenValid(string loginName, string currentSessionToken)
        {
            SessionLogin sessionLogin = await _sessionLoginCollection.Find(s => s.ALoginName == loginName).FirstOrDefaultAsync();
            if (sessionLogin != null)
            {
                if (currentSessionToken == sessionLogin.AToken)
                {
                    return true;
                }
            }
            return false;
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private UserAgent GetInfoUserAgent()
        {
            string userAgent = _accessor.HttpContext.Request.Headers["User-Agent"].ToString();
            string[] userAgentSplit = userAgent.Split(" ");
            string aBrowser = "";
            string aBrowserName = "";
            string aBrowserVersion = "";
            string[] tempUserAgent = null;

            if (!userAgentSplit[userAgentSplit.Length - 1].Contains("Safari"))
            {
                tempUserAgent = userAgentSplit[userAgentSplit.Length - 1].Split("/");
            }
            else
            {
                tempUserAgent = userAgentSplit[userAgentSplit.Length - 2].Split("/");
            }

            aBrowserName = tempUserAgent[0];
            aBrowserVersion = tempUserAgent[1].Split(".")[0] + "." + tempUserAgent[1].Split(".")[1];
            aBrowser = aBrowserName + aBrowserVersion;

            return new UserAgent()
            {
                UserAgentString = userAgent,
                Browser = aBrowser,
                BrowserName = aBrowserName,
                BrowserVersion = aBrowserVersion,
            };
        }
    }
}
