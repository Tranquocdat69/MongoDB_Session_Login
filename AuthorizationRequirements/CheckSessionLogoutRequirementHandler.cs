using Microsoft.AspNetCore.Authorization;

namespace MongoDB_Session_Login.AuthorizationRequirements
{
    public class CheckSessionLogoutRequirementHandler : AuthorizationHandler<CheckSessionLogoutRequirement>
    {
        private readonly IHttpContextAccessor _accessor;

        public CheckSessionLogoutRequirementHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckSessionLogoutRequirement requirement)
        {
            var currentUserLogin = _accessor.HttpContext.Request.Cookies["session_login"];
            
            if (!String.IsNullOrEmpty(currentUserLogin))
            {
                context.Succeed(requirement);
            }
            
            return Task.CompletedTask;
        }
    }
}
