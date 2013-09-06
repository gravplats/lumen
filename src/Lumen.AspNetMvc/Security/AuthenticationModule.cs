using System;
using System.Linq;
using System.Web;
using Lumen.AspNetMvc.Security.AppHarbor;
using Lumen.Security;

namespace Lumen.AspNetMvc.Security
{
    public abstract class AuthenticationModule<TUser> : IHttpModule
        where TUser : class, IUser
    {
        private static readonly ConfigFileAuthenticationConfiguration Configuration = new ConfigFileAuthenticationConfiguration();

        public void Dispose() { }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnAuthenticateRequest;
            context.EndRequest += OnEndRequest;
        }

        private void OnAuthenticateRequest(object sender, EventArgs e)
        {
            var httpApplication = (HttpApplication)sender;
            var httpContext = new HttpContextWrapper(httpApplication.Context);

            CreateAuthentication(httpContext).Authenticate();
        }

        protected abstract ApplicationAuthentication<TUser> CreateAuthentication(HttpContextBase httpContext);

        private static void OnEndRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            var response = context.Response;

            if (response.Cookies.Keys.Cast<string>().Contains(Configuration.CookieName))
            {
                response.Cache.SetCacheability(HttpCacheability.NoCache, "Set-Cookie");
            }

            if (response.StatusCode == 401 && !context.Request.QueryString.AllKeys.Contains("returnUrl"))
            {
                var delimiter = "?";
                var loginUrl = Configuration.LoginUrl;
                if (loginUrl.Contains("?"))
                {
                    delimiter = "&";
                }

                response.Redirect(loginUrl + delimiter + "returnUrl=" + HttpUtility.UrlEncode(context.Request.RawUrl), false);
            }
        }
    }
}