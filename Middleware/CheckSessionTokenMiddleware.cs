using MongoDB_Session_Login.Models.Session;
using MongoDB_Session_Login.Services;

namespace MongoDB_Session_Login.Middleware
{
    public class CheckSessionTokenMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly SessionLoginService _sessionLoginService;

        public CheckSessionTokenMiddleware(RequestDelegate requestDelegate, SessionLoginService sessionLoginService)
        {
            _requestDelegate = requestDelegate;
            _sessionLoginService = sessionLoginService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.ToString().Contains("v1"))
            {
                var currentLoginName = httpContext.Session.GetString("current_user_login");
                var currentSessionToken = httpContext.Session.GetString("current_session_token");

                if (!String.IsNullOrEmpty(currentLoginName) && !String.IsNullOrEmpty(currentSessionToken))
                {
                    bool isValidSessionToken = await _sessionLoginService.IsSessionTokenValid(currentLoginName, currentSessionToken);
                    if (isValidSessionToken)
                    {
                        await _requestDelegate(httpContext);
                    }
                    else
                    {
                        await httpContext.Response.WriteAsync(new UnauthorizedAccessException().ToString());
                    }
                }
                else
                {
                    await httpContext.Response.WriteAsync(new UnauthorizedAccessException().ToString());
                }
            }
            else
            {
                await _requestDelegate(httpContext);
            }
        }
    }
}
