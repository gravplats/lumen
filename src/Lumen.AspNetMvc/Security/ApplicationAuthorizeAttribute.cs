using System.Web;
using System.Web.Mvc;

namespace Lumen.AspNetMvc.Security
{
    public abstract class ApplicationAuthorizeAttribute : AuthorizeAttribute
    {
        private ActionDescriptor actionDescriptor;

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            actionDescriptor = filterContext.ActionDescriptor;
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return CreateAuthentication(httpContext).Authenticate() || AllowAnonymous();
        }

        private bool AllowAnonymous()
        {
            return actionDescriptor.IsDefined(typeof(ApplicationAllowAnonymousAttribute), inherit: true) ||
                actionDescriptor.ControllerDescriptor.IsDefined(typeof(ApplicationAllowAnonymousAttribute), inherit: true);
        }

        protected abstract IApplicationAuthentication CreateAuthentication(HttpContextBase httpContext);
    }
}