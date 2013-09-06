using System;
using System.Web;
using Lumen.AspNetMvc.Security.AppHarbor;
using Lumen.Security;

namespace Lumen.AspNetMvc.Security
{
    public class AuthenticationToken : IAuthenticationToken
    {
        private static readonly ConfigFileAuthenticationConfiguration Configuration = new ConfigFileAuthenticationConfiguration();

        public void Clear(IUser user)
        {
            user.Revoke();
            RemoveCookie();
        }

        public Guid Generate(string username)
        {
            Guid authenticationToken;

            var httpCookie = CreateCookie(username, out authenticationToken);
            HttpContext.Current.Response.Cookies.Add(httpCookie);

            return authenticationToken;
        }

        public static HttpCookie CreateCookie(string username, out Guid authenticationToken, bool persistent = false)
        {
            using (var protector = new CookieProtector(Configuration))
            {
                authenticationToken = Guid.NewGuid();

                var authenticationCookie = new AuthenticationCookie(0, authenticationToken, persistent, username);
                return authenticationCookie.CreateHttpCookie(protector, Configuration);
            }
        }

        public static void RemoveCookie()
        {
            var response = HttpContext.Current.Response;

            response.Cookies.Remove(Configuration.CookieName);
            response.Cookies.Add(new HttpCookie(Configuration.CookieName, "")
            {
                Expires = DateTime.UtcNow.AddMonths(-100),
            });
        }
    }
}