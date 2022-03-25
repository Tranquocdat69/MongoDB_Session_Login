using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB_Test.Models.Session;
using MongoDB_Test.Models.SessionLogin;
using MongoDB_Test.Services;

namespace MongoDB_Test.Controllers
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

        [HttpGet("GetAllSessionLogin")]
        public async Task<List<SessionLogin>> GetAll()
        {
            return await _sessionLoginService.GetAllAsync();
        }

        [HttpGet("GetSessionLoginFromIPAddress/{ipAddress}")]
        public async Task<List<SessionLogin>> GetSessionLoginFromIPAddress(string ipAddress) =>
            await _sessionLoginService.GetSessionLoginFromIPAdressAsync(ipAddress);

        [HttpGet("GetSessionLoginByAccount/{loginName}")]
        public async Task<SessionLogin> Get(string loginName) => 
            await _sessionLoginService.GetAsync(loginName);

        [HttpPost("AddAccount")]
        public async Task<IActionResult> Create([FromBody] SessionLoginName sessionLoginName)
        {
            var isSessionLoginExisted = await _sessionLoginService.GetAsync(sessionLoginName.LoginName) is not null;
            if (!isSessionLoginExisted)
            {
                await _sessionLoginService.CreateAsync(sessionLoginName.LoginName, sessionLoginName.IPAddress);

                return NoContent();
            }
            else
            {
                return Conflict("Already existed login name"); 
            }
        }

        [HttpPut("UpdateTokenSession/{loginName}")]
        public async Task<IActionResult> UpdateTokenSession(string loginName)
        {
            var sessionLogin = await _sessionLoginService.GetAsync(loginName);

            if (sessionLogin is null)
            {
                return NotFound();
            }

            await _sessionLoginService.UpdateTokenAsync(loginName, Guid.NewGuid().ToString());

            return NoContent();
        }

        [HttpDelete("DeleteAccount/{loginName}")]
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

        [HttpDelete("DeleteAll")]
        public async Task<ActionResult> DeleteAll()
        {
            await _sessionLoginService.DeleteAllAsync();

            return NoContent();
        }
    }
}
