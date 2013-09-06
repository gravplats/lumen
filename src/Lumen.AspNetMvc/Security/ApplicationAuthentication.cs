using System;
using System.Web;
using Lumen.AspNetMvc.Security.AppHarbor;
using Lumen.Security;

namespace Lumen.AspNetMvc.Security
{
    public abstract class ApplicationAuthentication<TUser> : IApplicationAuthentication
        where TUser : class, IUser
    {
        private static readonly ConfigFileAuthenticationConfiguration Configuration = new ConfigFileAuthenticationConfiguration();

        private readonly HttpContextBase httpContext;

        protected ApplicationAuthentication(HttpContextBase httpContext)
        {
            this.httpContext = Ensure.NotNull(httpContext, "httpContext");
        }

        public bool Authenticate()
        {
            var cookie = httpContext.Request.Cookies[Configuration.CookieName];
            if (cookie != null)
            {
                var protector = new CookieProtector(Configuration);
                try
                {
                    byte[] data;
                    if (protector.Validate(cookie.Value, out data))
                    {
                        var authenticationCookie = AuthenticationCookie.Deserialize(data);
                        if (authenticationCookie.IsExpired(Configuration.Timeout))
                        {
                            return false;
                        }

                        var principal = authenticationCookie.GetPrincipal();

                        var identity = principal.Identity as CookieIdentity;
                        if (identity == null)
                        {
                            return false;
                        }

                        var user = GetUser(httpContext, identity.Name);
                        if (user != null && user.AuthenticationToken == identity.AuthenticationToken)
                        {
                            httpContext.User = ApplicationPrincipal<TUser>.Create(user);
                            RenewCookieIfExpiring(httpContext, protector, authenticationCookie);
                        }
                    }

                    return true;
                }
                catch
                {
                    // do not leak any information if an exception was thrown; simply don't set the context.User property.
                }
                finally
                {
                    protector.Dispose();
                }
            }

            return false;
        }

        protected abstract TUser GetUser(HttpContextBase httpContext, string username);

        private static void RenewCookieIfExpiring(HttpContextBase context, CookieProtector protector, AuthenticationCookie authenticationCookie)
        {
            if (!Configuration.SlidingExpiration || !authenticationCookie.IsExpired(TimeSpan.FromTicks(Configuration.Timeout.Ticks / 2)))
            {
                return;
            }

            authenticationCookie.Renew();
            context.Response.Cookies.Remove(Configuration.CookieName);

            var newCookie = authenticationCookie.CreateHttpCookie(protector, Configuration);
            context.Response.Cookies.Add(newCookie);
        }
    }
}