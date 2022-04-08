using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB_Session_Login.Models;
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

        [HttpPost("FakeLogin")]
        public async Task<IActionResult> FakeLogin([FromBody] SessionLoginName sessionLoginName)
        {
            SessionLogin sessionLogin = await _sessionLoginService.GetAsync(sessionLoginName.ALoginName);
            if (sessionLogin is not null)
            {
                SessionLogin newSessionLogin = await _sessionLoginService.UpdateWhenLoginAsync(sessionLoginName.ALoginName);
                Response.Cookies.Append("session_token", newSessionLogin.AToken, CustomCookieOptions.option);
                var userClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, sessionLoginName.ALoginName),
                    new Claim(ClaimTypes.Email, "demo@gmail.com"),
                    new Claim(ClaimTypes.DateOfBirth, "01/01/1000"),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var userIdentity = new ClaimsIdentity(userClaims, "user identity");
                var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });
                await HttpContext.SignInAsync(userPrincipal);

                return Ok(newSessionLogin);
            }
            else
            {
                await _sessionLoginService.CreateAsync(sessionLoginName.ALoginName);
                return Ok("Created new session login");
            }
        }

        [HttpGet("RequireLogin")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult RequireLogin()
        {
            return Unauthorized();
        }

        [HttpPost("FakeLogout")]
        [Authorize(Policy = "CheckLogout")]
        public async Task<IActionResult> FakeLogout()
        {
            if (!String.IsNullOrEmpty(HttpContext.User.Identity.Name))
            {
                Response.Cookies.Delete("session_token", CustomCookieOptions.option);
                await HttpContext.SignOutAsync();
                await _sessionLoginService.UpdateLogoutTimeAsync(HttpContext.User.Identity.Name);
            }
            else
            {
                await _sessionLoginService.UpdateLogoutTimeAsync(null);
            }
                return Ok();
        }

        [HttpGet("v1/GetAllSessionLogin")]
        [Authorize(Policy = "CheckSessionToken")]
        public async Task<List<SessionLogin>> GetAll()
        {
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

     /*   [HttpPost("v1/AddAccount")]
        [Authorize(Policy = "CheckSessionToken")]
        public async Task<IActionResult> Create([FromBody] SessionLoginName sessionLoginName)
        {
            var isSessionLoginExisted = await _sessionLoginService.GetAsync(sessionLoginName.ALoginName) is not null;
            if (!isSessionLoginExisted)
            {
                await _sessionLoginService.CreateAsync(sessionLoginName.ALoginName);

                return Ok();
            }
            else
            {
                return Conflict("Already existed login name");
            }
        }*/

        [HttpDelete("v1/DeleteAccount/{loginName}")]
        [Authorize(Policy = "CheckSessionToken")]
        public async Task<ActionResult> Delete(string loginName)
        {
            var sessionLogin = await _sessionLoginService.GetAsync(loginName);
            if (sessionLogin == null)
            {
                return NotFound("Account does not exist");
            }
            await _sessionLoginService.DeleteAsync(loginName);

            return NoContent();
        }

        [HttpDelete("v1/DeleteAll")]
        [Authorize(Policy = "CheckSessionToken")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> DeleteAll()
        {
            await _sessionLoginService.DeleteAllAsync();

            return NoContent();
        }
    }
}
