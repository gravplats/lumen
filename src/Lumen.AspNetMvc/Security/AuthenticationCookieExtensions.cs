using System.Web;
using Lumen.AspNetMvc.Security.AppHarbor;

namespace Lumen.AspNetMvc.Security
{
    public static class AuthenticationCookieExtensions
    {
        public static HttpCookie CreateHttpCookie(this AuthenticationCookie authenticationCookie, CookieProtector protector, ConfigFileAuthenticationConfiguration configuration)
        {
            var cookie = new HttpCookie(configuration.CookieName, protector.Protect(authenticationCookie.Serialize()))
            {
                HttpOnly = true,
                Secure = configuration.RequireSSL,
            };

            if (authenticationCookie.Persistent)
            {
                cookie.Expires = authenticationCookie.IssueDate + configuration.Timeout;
            }

            return cookie;
        }
    }
}