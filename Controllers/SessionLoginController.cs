using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB_Session_Login.Models.Session;
using MongoDB_Session_Login.Models.SessionLogin;
using MongoDB_Session_Login.Services;
using System.Net;
using System.Security.Claims;

namespace MongoDB_Session_Login.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionLoginController : ControllerBase
    {
        private readonly SessionLoginService _sessionLoginService;

        public SessionLoginController(SessionLoginService sessionLoginService)
        {
            _sessionLoginService = sessionLoginService;
        }

        [HttpGet("FakeLogin/{loginName}")]
        public async Task<IActionResult> FakeLogin(string loginName)
        {
            SessionLogin sessionLogin = await _sessionLoginService.GetAsync(loginName);
            if (sessionLogin is not null)
            {
                CookieOptions option = new CookieOptions();
                option.Domain = "fpts.com.vn";
                option.SameSite = SameSiteMode.Strict;
                option.Secure = true;
                option.HttpOnly = false;
                SessionLogin newSessionLogin = await _sessionLoginService.UpdateTokenAsync(loginName);
                Response.Cookies.Append("session_token", newSessionLogin.TokenSession, option);
                /* HttpContext.Session.SetString("current_user_login", loginName);
                 HttpContext.Session.SetString("current_session_token", newSessionToken);*/
                var userClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, loginName),
                    new Claim(ClaimTypes.Email, "demo@gmail.com"),
                    new Claim(ClaimTypes.DateOfBirth, "01/01/1000"),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var userIdentity = new ClaimsIdentity(userClaims, "user identity");
                var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });
                await HttpContext.SignInAsync(userPrincipal);
                var name = User.Claims.FirstOrDefault();
                return Ok(newSessionLogin);
            }

            return NotFound("Login Name does not exist");
        }

        [HttpGet("RequireLogin")]
        public IActionResult RequireLogin()
        {
            return Unauthorized();
        }

        [HttpGet("FakeLogout")]
        [Authorize] 
        public IActionResult FakeLogout()
        {
            Response.Cookies.Delete("session_token");
            HttpContext.SignOutAsync();
            //Response.Cookies.Delete("session_login");

            return Ok();
        }

        /* [HttpGet("GetSessionTokeFromCookie")]
         [Authorize]
         public IActionResult Get()
         {
             return Ok(Request.Cookies["session_token"]);
         }*/
        /*
                [HttpGet("IsSessionTokenValid/{loginName}/{currentSessionToken}")]
                public async Task<bool> IsSessionTokenValid(string loginName, string currentSessionToken) =>
                    await _sessionLoginService.IsSessionTokenValid(loginName, currentSessionToken);*/

        [HttpGet("v1/GetAllSessionLogin")]
        [Authorize(Policy = "CheckSessionToken")]
        public async Task<List<SessionLogin>> GetAll()
        {
            /*if (!String.IsNullOrEmpty(HttpContext.Session.GetString("current_session_token")))
            {
                Console.WriteLine(HttpContext.Session.GetString("current_user_login"));
                Console.WriteLine(HttpContext.Session.GetString("current_session_token"));
            }*/

            return await _sessionLoginService.GetAllAsync();
        }

        [HttpGet("v1/GetSessionLoginFromIPAddress/{ipAddress}")]
        [Authorize(Policy = "CheckSessionToken")]
        public async Task<List<SessionLogin>> GetSessionLoginFromIPAddress(string ipAddress) =>
            await _sessionLoginService.GetSessionLoginFromIPAdressAsync(ipAddress);

        [HttpGet("v1/GetSessionLoginByAccount/{loginName}")]
        [Authorize(Policy = "CheckSessionToken")]
        public async Task<SessionLogin> Get(string loginName) =>
            await _sessionLoginService.GetAsync(loginName);

        [HttpPost("v1/AddAccount")]
        [Authorize(Policy = "CheckSessionToken")]
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
        [Authorize(Policy = "CheckSessionToken")]
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
        [Authorize(Policy = "CheckSessionToken")]
        public async Task<ActionResult> DeleteAll()
        {
            await _sessionLoginService.DeleteAllAsync();

            return NoContent();
        }
    }
}
