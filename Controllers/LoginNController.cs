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
        private readonly OracleContext _context;
        public LoginNController(OracleContext context, SessionLoginService sessionLoginService)
        {
            _context = context;
            _sessionLoginService = sessionLoginService;
        }

        [HttpPost("/Register")]
        public async Task<IActionResult> Register(Login loginCache)
        {
            CheckLogin login = new CheckLogin();
            Permit permit = new Permit();
            UserTest user = new UserTest();

            user.ALOGINNAME = loginCache.User.ALOGINNAME;
            user.ATOKEN = loginCache.User.ATOKEN;
            user.AIPSERVER = loginCache.User.AIPSERVER;
            user.AIPCLIENT = loginCache.User.AIPCLIENT;
            user.AUSERAGENT = loginCache.User.AUSERAGENT;
            user.ABROWSER = loginCache.User.ABROWSER;
            user.ALOGINTIME = loginCache.User.ALOGINTIME;
            user.ALOGOUTTIME = loginCache.User.ALOGOUTTIME;
            user.ASESSIONNO = loginCache.User.ASESSIONNO;
            user.ASESSIONFIRSTLOGIN = loginCache.User.ASESSIONFIRSTLOGIN;
            user.AISMOBILE = loginCache.User.AISMOBILE;
            user.ABROWSERNAME = loginCache.User.ABROWSERNAME;
            user.ABROWSERVERS = loginCache.User.ABROWSERVERS;
            user.ACHKPASS2 = loginCache.User.ACHKPASS2;
            _context.Add(user);
            _context.SaveChanges();

            permit.EzTradeChargeRate = loginCache.Permit.EzTradeChargeRate;
            permit.EzTrade = loginCache.Permit.EzTrade;
            permit.EzTransfer = loginCache.Permit.EzTransfer;
            permit.EzAdvance = loginCache.Permit.EzAdvance;
            permit.EzMargin = loginCache.Permit.EzMargin;
            permit.EzMortgage = loginCache.Permit.EzMortgage;
            permit.EzOddlot = loginCache.Permit.EzOddlot;
            permit.EzMarginPro = loginCache.Permit.EzMarginPro;
            permit.EzFuture = loginCache.Permit.EzFuture;
            permit.EzTvdt = loginCache.Permit.EzTvdt;
            permit.vTblid = loginCache.Permit.vTblid;
            permit.vFeeUP = loginCache.Permit.vFeeUP;
            permit.vFeeUP_CCQ = loginCache.Permit.vFeeUP_CCQ;
            permit.vFeeLISTED_CP = loginCache.Permit.vFeeLISTED_CP;
            permit.vFeeHSX_CP = loginCache.Permit.vFeeHSX_CP;
            permit.vFeeRate_TP = loginCache.Permit.vFeeRate_TP;
            permit.vFeeLISTED_ETF = loginCache.Permit.vFeeLISTED_ETF;
            permit.vFeeLISTED_CCQ = loginCache.Permit.vFeeLISTED_CCQ;
            permit.vFeeHSX_CCQ = loginCache.Permit.vFeeHSX_CCQ;
            permit.vFeeHSX_CQ = loginCache.Permit.vFeeHSX_CQ;
            permit.vFeeHSX_ETF = loginCache.Permit.vFeeHSX_ETF;
            permit.vFeeLISTED_CQ = loginCache.Permit.vFeeLISTED_CQ;
            permit.UserId = user.ID;

            login.Time = DateTime.Now.ToString();
            login.UserId = user.ID;
            _context.Add(login);
            _context.Add(permit);
            _context.SaveChanges();


            await _sessionLoginService.CreateAsync(loginCache.User.ALOGINNAME, HttpContext.Connection.RemoteIpAddress.ToString());


            return Ok(user);
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
                var user = _context.UserLogin.Where(s => s.ALOGINNAME.Equals(login.UserName) && s.ACHKPASS2.Equals(login.Password)).FirstOrDefault();
                // ).FirstOrDefault(

                if (user != null)
                {
                    var result = from u in _context.UserLogin
                                 join p in _context.Permits on u.ID equals p.UserId
                                 join r in _context.ResponseLogin on u.ID equals r.UserId
                                 where u.ALOGINNAME == login.UserName && u.ACHKPASS2 == login.Password
                                 select new Login
                                 {
                                     Time = r.Time,
                                     User = u,
                                     Permit = p,

                                 };
                    SessionLogin sessionLogin = await _sessionLoginService.GetAsync(login.UserName);
                    if (sessionLogin is not null){
                        var tokenNow = Guid.NewGuid().ToString();
                        await _sessionLoginService.UpdateSessionToken(login.UserName,tokenNow);
                        result.First().User.ATOKEN = tokenNow;
                    }

                    

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
