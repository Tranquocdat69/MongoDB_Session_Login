using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB_Session_Login.Data;
using MongoDB_Session_Login.Models.LoginForLongPv;
using MongoDB_Session_Login.Models.Session;
using MongoDB_Session_Login.Models.SessionLogin;
using MongoDB_Session_Login.Services;

namespace MongoDB_Session_Login.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionLoginController : ControllerBase
    {
        private readonly SessionLoginService _sessionLoginService;
        private readonly OracleContext _oracleContext;

        public SessionLoginController(SessionLoginService sessionLoginService, OracleContext oracleContext)
        {
            _sessionLoginService = sessionLoginService;
            _oracleContext = oracleContext;
        }
        [HttpPost("RealLogin")]
        public async Task<IActionResult> RealLogin(FormLogin login)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(login.UserName))
                {
                    return BadRequest("fail");
                }
                var user = _oracleContext.UserLogin.Where(s => s.ALOGINNAME.Equals(login.UserName) && s.ACHKPASS2.Equals(login.Password)).FirstOrDefault();

                if (user != null)
                {
                    var result = from u in _oracleContext.UserLogin
                                 join p in _oracleContext.Permits on u.ID equals p.UserId
                                 join r in _oracleContext.ResponseLogin on u.ID equals r.UserId
                                 where u.ALOGINNAME == login.UserName && u.ACHKPASS2 == login.Password
                                 select new Login
                                 {
                                     Time = r.Time,
                                     User = u,
                                     Permit = p,

                                 };
                    SessionLogin sessionLogin = await _sessionLoginService.GetAsync(login.UserName);
                    if (sessionLogin is null)
                    {
                        await _sessionLoginService.CreateAsync(login.UserName, HttpContext.Connection.RemoteIpAddress.ToString());
                        //update token mongoDB
                        string newSessionToken = await _sessionLoginService.UpdateTokenAsync(login.UserName);
                        //session server 
                        HttpContext.Session.SetString("current_user_login", login.UserName);
                        HttpContext.Session.SetString("current_session_token", newSessionToken);

                        //return Ok(sessionLogin);
                    }
                    else
                    {
                        //update token mongoDB
                        string newSessionToken = await _sessionLoginService.UpdateTokenAsync(login.UserName);
                        //session server 
                        
                        HttpContext.Session.SetString("current_user_login", login.UserName);
                        HttpContext.Session.SetString("current_session_token", newSessionToken);

                    }

                    return Ok (result);
                }
                else
                {
                    return BadRequest("fail");
                }
            }
            return NotFound("Login Name does not exist");
        }
        /*
                [HttpGet("IsSessionTokenValid/{loginName}/{currentSessionToken}")]
                public async Task<bool> IsSessionTokenValid(string loginName, string currentSessionToken) =>
                    await _sessionLoginService.IsSessionTokenValid(loginName, currentSessionToken);*/

        [HttpGet("v1/GetAllSessionLogin")]
        public async Task<List<SessionLogin>> GetAll()
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("current_session_token")))
            {
                Console.WriteLine(HttpContext.Session.GetString("current_user_login"));
                Console.WriteLine(HttpContext.Session.GetString("current_session_token"));
            }

            return await _sessionLoginService.GetAllAsync();
        }

        [HttpGet("v1/GetSessionLoginFromIPAddress/{ipAddress}")]
        public async Task<List<SessionLogin>> GetSessionLoginFromIPAddress(string ipAddress) =>
            await _sessionLoginService.GetSessionLoginFromIPAdressAsync(ipAddress);

        [HttpGet("v1/GetSessionLoginByAccount/{loginName}")]
        public async Task<SessionLogin> Get(string loginName) =>
            await _sessionLoginService.GetAsync(loginName);

        [HttpPost("v1/AddAccount")]
        public async Task<IActionResult> Create([FromBody] SessionLoginName sessionLoginName)
        {
            var isSessionLoginExisted = await _sessionLoginService.GetAsync(sessionLoginName.LoginName) is not null;
            if (!isSessionLoginExisted)
            {
                await _sessionLoginService.CreateAsync(sessionLoginName.LoginName, HttpContext.Connection.RemoteIpAddress.ToString());

                return Ok();
            }
            else
            {
                return Conflict("Already existed login name");
            }
        }

        /* [HttpPut("v1/UpdateTokenSession/{loginName}")]
         public async Task<IActionResult> UpdateTokenSession(string loginName)
         {
             var sessionLogin = await _sessionLoginService.GetAsync(loginName);

             if (sessionLogin is null)
             {
                 return NotFound();
             }

             await _sessionLoginService.UpdateTokenAsync(loginName);

             return NoContent();
         }*/

        [HttpDelete("v1/DeleteAccount/{loginName}")]
        public async Task<ActionResult> Delete(string loginName)
        {
            var sessionLogin = await _sessionLoginService.GetAsync(loginName);
            if (sessionLogin == null)
            {
                return NotFound();
            }
            await _sessionLoginService.DeleteAsync(loginName);

            return NoContent();
        }

        [HttpDelete("v1/DeleteAll")]
        public async Task<ActionResult> DeleteAll()
        {
            await _sessionLoginService.DeleteAllAsync();

            return NoContent();
        }
    }
}
