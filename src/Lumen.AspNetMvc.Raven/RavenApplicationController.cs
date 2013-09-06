using System.Web.Mvc;
using Lumen.Security;
using Raven.Client;

namespace Lumen.AspNetMvc.Raven
{
    public abstract class RavenApplicationController<TUser> : ApplicationController<TUser>
        where TUser : class, IUser
    {
        protected IDocumentSession RavenSession { get; set; }

        protected abstract string RavenSessionKey { get; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // OnActionExecuting is (obviously) run after the controller constructor, thus it's not possible to use
            // 'Transaction' in the constructor, ie. pass it along as a constructor argument to another component.
            RavenSession = (IDocumentSession)filterContext.HttpContext.Items[RavenSessionKey];
        }

        protected override TResult InvokeService<TService, TPayload, TResult>(TPayload payload, TUser user)
        {
            var invoker = DependencyResolver.Current.GetService<ApplicationServiceInvoker<RavenApplicationServiceContext>>();
            return invoker.Invoke<TService, TResult>(new RavenApplicationServiceContext(payload, user, RavenSession));
        }
    }

    public abstract class RavenApplicationController : RavenApplicationController<LumenUser> { }
}