using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB_Session_Login.Data;
using MongoDB_Session_Login.Models.LoginForLongPv;
using MongoDB_Session_Login.Models.Session;
using MongoDB_Session_Login.Services;
using Newtonsoft.Json;

namespace MongoDB_Session_Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginNController : ControllerBase
    {
        private readonly SessionLoginService _sessionLoginService;
        private readonly TAuthContext _context;
        private readonly HashService _hashService;
        public LoginNController(TAuthContext context, SessionLoginService sessionLoginService,HashService hashService)
        {
            _hashService = hashService;
            _context = context;
            _sessionLoginService = sessionLoginService;
        }

        [HttpPost("/Register")]
        public async Task<IActionResult> Register(Login loginCache)
        {
            if (ModelState.IsValid)
            {
                var checkUser = _context.TAUTH_USERLOGIN.Any(f => f.ACLIENTCODE == loginCache.User.ACLIENTCODE);

                if (checkUser == false)
                {
                    var hashPassword = _hashService.MD5Hash(loginCache.User.APASSWORD);
                    var hashPassword2 = _hashService.MD5Hash(loginCache.User.APASSWORDII);

                    TauthClientsession clientSession = new TauthClientsession();
                    TauthClientsessionlog clientSessionLog = new TauthClientsessionlog();
                    TauthUserlogin user = new TauthUserlogin();

                    user.ACLIENTCODE = loginCache.User.ACLIENTCODE;
                    user.ACLIENTNAME = loginCache.User.ACLIENTNAME;
                    user.APASSWORD = hashPassword;
                    user.APASSWORDII = hashPassword2;
                    user.ALASTPASSWORDCHANGEDDATE = loginCache.User.ALASTPASSWORDCHANGEDDATE;
                    user.ARSATOKEN = loginCache.User.ARSATOKEN;
                    user.ACLIENTLOCKSTATUS = loginCache.User.ACLIENTLOCKSTATUS;
                    user.ACLIENTLOCKTIME = loginCache.User.ACLIENTLOCKTIME;
                    user.ADESCRIPTION = loginCache.User.ADESCRIPTION;
                    user.AFIRSTLOGIN = loginCache.User.AFIRSTLOGIN;
                    user.ARETRYCOUNT = loginCache.User.ARETRYCOUNT;
                    user.AUSINGPWD1BY1 = loginCache.User.AUSINGPWD1BY1;
                    user.AMOBILEDEVICEID = loginCache.User.AMOBILEDEVICEID;
                    user.AREASON = loginCache.User.AREASON;

                    //Add UserLogin
                    _context.TAUTH_USERLOGIN.Add(user);

                    clientSession.ALOGINNAME = loginCache.User.ACLIENTCODE;
                    clientSession.ATOKEN = loginCache.ClientSession.ATOKEN;
                    clientSession.AIPSERVER = loginCache.ClientSession.AIPSERVER;
                    clientSession.AIPCLIENT = loginCache.ClientSession.AIPCLIENT;
                    clientSession.AUSERAGENT = loginCache.ClientSession.AUSERAGENT;
                    clientSession.ABROWSER = loginCache.ClientSession.ABROWSER;
                    clientSession.ALOGINTIME = loginCache.ClientSession.ALOGINTIME;
                    clientSession.ALOGOUTTIME = loginCache.ClientSession.ALOGOUTTIME;
                    clientSession.ASESSIONNO = loginCache.ClientSession.ASESSIONNO;
                    clientSession.ASESSIONFIRSTLOGIN = loginCache.ClientSession.ASESSIONFIRSTLOGIN;
                    clientSession.AISMOBILE = loginCache.ClientSession.AISMOBILE;
                    clientSession.ABROWSERNAME = loginCache.ClientSession.ABROWSERNAME;
                    clientSession.ABROWSERVERS = loginCache.ClientSession.ABROWSERVERS;
                    clientSession.ACHKPASS2 = loginCache.ClientSession.ACHKPASS2;

                    //Add ClientSession
                    _context.TAUTH_CLIENTSESSION.Add(clientSession);

                    clientSessionLog.ATBLID = loginCache.ClientSessionLog.ATBLID;
                    clientSessionLog.ALOGINNAME = loginCache.User.ACLIENTCODE;
                    clientSessionLog.AERRCODE = loginCache.ClientSessionLog.AERRCODE;
                    clientSessionLog.AERRMESSAGE = loginCache.ClientSessionLog.AERRMESSAGE;
                    clientSessionLog.ASOURCE = loginCache.ClientSessionLog.ASOURCE;
                    clientSessionLog.AIPSERVER = loginCache.ClientSessionLog.AIPSERVER;
                    clientSessionLog.AIPCLIENT = loginCache.ClientSessionLog.AIPCLIENT;
                    clientSessionLog.AREFERER = loginCache.ClientSessionLog.AREFERER;
                    clientSessionLog.AUSERAGENT = loginCache.ClientSessionLog.AUSERAGENT;
                    clientSessionLog.ABROWSER = loginCache.ClientSessionLog.ABROWSER;
                    clientSessionLog.ALOGTIME = loginCache.ClientSessionLog.ALOGTIME;
                    clientSessionLog.AACTIVITY = loginCache.ClientSessionLog.AACTIVITY;
                    clientSessionLog.AACTIVITYUSR = loginCache.ClientSessionLog.AACTIVITYUSR;
                    clientSessionLog.AACTIVITYDSC = loginCache.ClientSessionLog.AACTIVITYDSC;
                    clientSessionLog.AACTIVITYBTNTYPE = loginCache.ClientSessionLog.AACTIVITYBTNTYPE;
                    clientSessionLog.AISMOBILE = loginCache.ClientSessionLog.AISMOBILE;
                    clientSessionLog.ABROWSERNAME = loginCache.ClientSessionLog.ABROWSERNAME;
                    clientSessionLog.ABROWSERVERS = loginCache.ClientSessionLog.ABROWSERVERS;
                    clientSessionLog.ABRKID = loginCache.ClientSessionLog.ABRKID;

                    //_context.Add(user);
                    //_context.Add(clientSession);

                    //Add ClientSessionLog
                    _context.TAUTH_CLIENTSESSIONLOG.Add(clientSessionLog);
                    _context.SaveChanges();

                    //await _sessionLoginService.CreateAsync(loginCache.User., HttpContext.Connection.RemoteIpAddress.ToString());
                    return Ok(user);
                }
                else
                {
                    return BadRequest("Tai khoan da ton tai");
                }
            }
            else
            {
                return BadRequest("Input sai");
            }
               
        }

        [HttpPost("/Login")]
        public async Task<IActionResult> Login(FormLogin login)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(login.UserName))
                {
                    return BadRequest("fail");
                }
                var passwordHash = _hashService.MD5Hash(login.Password);

                //var user = _context.TAUTH_USERLOGIN.Where(s => s.ACLIENTCODE.Equals(login.UserName) && s.APASSWORD.Equals(passwordHash)).FirstOrDefault();

                var user = _context.TAUTH_USERLOGIN.Any(f => f.ACLIENTCODE == login.UserName && f.APASSWORD == passwordHash);
                if (user == true)
                {
                    var result = from u in _context.TAUTH_USERLOGIN
                                 join p in _context.TAUTH_CLIENTSESSION on u.ACLIENTCODE equals p.ALOGINNAME
                                 //join r in _context.ResponseLogin on u.ID equals r.UserId
                                 where u.ACLIENTCODE == login.UserName && u.APASSWORD == passwordHash
                                 select new Login
                                 {
                                     Time = DateTime.Now,
                                     User = u,
                                     ClientSession = p,
                                 };
                    //SessionLogin sessionLogin = await _sessionLoginService.GetAsync(login.UserName);
                    //if (sessionLogin is not null){
                    //    var tokenNow = Guid.NewGuid().ToString();
                    //    await _sessionLoginService.UpdateSessionToken(login.UserName,tokenNow);
                    //    result.First().User.ATOKEN = tokenNow;
                    //}

                    

                    return Ok( result);
                }
                else
                {
                    return BadRequest("fail");
                }
            }
            return BadRequest("fail");
        }
        //login vao thanh cong tao token , cung luc do save token xuong mongodb va tra token cho client save xuong cookie -> sau do se di check token giua client và mongodb 
    }
}
