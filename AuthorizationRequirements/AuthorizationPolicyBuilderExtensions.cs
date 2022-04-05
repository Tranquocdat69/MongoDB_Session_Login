using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MongoDB_Session_Login.AuthorizationRequirements
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireCustomCheckSessionToken(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new CheckSessionTokenRequirement());
            return builder;
        }
    }
}
