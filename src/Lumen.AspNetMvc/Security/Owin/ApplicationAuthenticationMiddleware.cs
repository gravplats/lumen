using System;
using System.Threading.Tasks;
using Lumen.AspNetMvc.Security.AppHarbor;
using Lumen.Security;
using Microsoft.Owin;

namespace Lumen.AspNetMvc.Security.Owin
{
    public abstract class ApplicationAuthenticationMiddleware<TUser> : OwinMiddleware
        where TUser : class, IUser
    {
        private static readonly ConfigFileAuthenticationConfiguration Configuration = new ConfigFileAuthenticationConfiguration();

        protected ApplicationAuthenticationMiddleware(OwinMiddleware next) : base(next) { }

        public override async Task Invoke(IOwinContext context)
        {
            await Authenticae(context);
            await Next.Invoke(context);
        }

        private async Task Authenticae(IOwinContext context)
        {
            var cookie = context.Request.Cookies[Configuration.CookieName];
            if (cookie == null)
            {
                return;
            }

            var protector = new CookieProtector(Configuration);
            try
            {
                byte[] data;
                if (!protector.Validate(cookie, out data))
                {
                    return;
                }

                var authenticationCookie = AuthenticationCookie.Deserialize(data);
                if (authenticationCookie.IsExpired(Configuration.Timeout))
                {
                    return;
                }

                var principal = authenticationCookie.GetPrincipal();

                var identity = principal.Identity as CookieIdentity;
                if (identity == null)
                {
                    return;
                }

                var user = await GetUser(context, identity.Name);
                if (user != null && user.AuthenticationToken == identity.AuthenticationToken)
                {
                    context.Request.User = ApplicationPrincipal<TUser>.Create(user);
                    RenewCookieIfExpiring(context.Response, protector, authenticationCookie);
                }
            }
            catch
            {
                // do not leak any information if an exception was thrown; simply don't set the IPrincipal.
            }
            finally
            {
                protector.Dispose();
            }
        }

        protected abstract Task<TUser> GetUser(IOwinContext context, string identifier);

        private static void RenewCookieIfExpiring(IOwinResponse response, CookieProtector protector, AuthenticationCookie authenticationCookie)
        {
            if (!Configuration.SlidingExpiration || !authenticationCookie.IsExpired(TimeSpan.FromTicks(Configuration.Timeout.Ticks / 2)))
            {
                return;
            }

            authenticationCookie.Renew();
            response.Cookies.Delete(Configuration.CookieName);

            var newCookie = authenticationCookie.CreateHttpCookie(protector, Configuration);
            response.Cookies.Append(Configuration.CookieName, newCookie.Value, new CookieOptions
            {
                Domain = newCookie.Domain,
                Expires = newCookie.Expires,
                HttpOnly = newCookie.HttpOnly,
                Path = newCookie.Path,
                Secure = newCookie.Secure
            });
        }
    }
}