using Microsoft.AspNetCore.Authorization;
using MongoDB_Session_Login.Services;
using System.Net;

namespace MongoDB_Session_Login.AuthorizationRequirements
{
    public class CheckSessionTokenRequirementHandler : AuthorizationHandler<CheckSessionTokenRequirement>
    {
        private readonly SessionLoginService _sessionLoginService;
        private readonly IHttpContextAccessor _accessor;


        public CheckSessionTokenRequirementHandler(IHttpContextAccessor accessor, SessionLoginService sessionLoginService)
        {
            _accessor = accessor;
            _sessionLoginService = sessionLoginService;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckSessionTokenRequirement requirement)
        {
            var currentUserLogin = _accessor.HttpContext.User.Identity.Name;
            var currentSessionToken = _accessor.HttpContext.Request.Cookies["session_token"];

            if (!String.IsNullOrEmpty(currentUserLogin) && !String.IsNullOrEmpty(currentSessionToken))
                {
                    bool isValidSessionToken = await _sessionLoginService.IsSessionTokenValid(currentUserLogin, currentSessionToken);
                    if (isValidSessionToken)
                    {
                       context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
        }
    }
}
