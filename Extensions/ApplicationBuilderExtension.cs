using MongoDB_Session_Login.Middleware;

namespace MongoDB_Session_Login.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseCheckSessionToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckSessionTokenMiddleware>();
        }
    }
}
