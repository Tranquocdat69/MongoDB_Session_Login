using Microsoft.AspNetCore.Authorization;
using MongoDB_Session_Login.AuthorizationRequirements;

namespace MongoDB_Session_Login.Extensions
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static AuthorizationPolicyBuilder RequireCustomCheckSessionToken(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new CheckSessionTokenRequirement());
            return builder;
        }
        
        public static AuthorizationPolicyBuilder RequireCustomCheckLogout(this AuthorizationPolicyBuilder builder)
        {
            builder.AddRequirements(new CheckSessionLogoutRequirement());
            return builder;
        }
    }
}
