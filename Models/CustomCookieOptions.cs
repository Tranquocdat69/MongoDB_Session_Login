namespace MongoDB_Session_Login.Models
{
    public class CustomCookieOptions
    {
        public static CookieOptions option = new CookieOptions()
        {
            Domain = "fpts.com.vn",
            SameSite = SameSiteMode.Lax,
            Secure = false,
            HttpOnly = true,
            Path = "/"
        };
    }
}
