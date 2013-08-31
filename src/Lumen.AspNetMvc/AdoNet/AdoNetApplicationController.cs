using System.Data;
using System.Web.Mvc;
using Lumen.AdoNet;
using Lumen.Security;

namespace Lumen.AspNetMvc.AdoNet
{
    public abstract class AdoNetApplicationController<TUser> : ApplicationController<TUser>
        where TUser : class, IUser
    {
        protected IDbTransaction Transaction { get; set; }

        protected abstract string TransactionKey { get; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // OnActionExecuting is (obviously) run after the controller constructor, thus it's not possible to use
            // 'Transaction' in the constructor, ie. pass it along as a constructor argument to another component.
            Transaction = (IDbTransaction)filterContext.HttpContext.Items[TransactionKey];
        }

        protected override TResult InvokeService<TService, TPayload, TResult>(TPayload payload, TUser user)
        {
            var invoker = DependencyResolver.Current.GetService<AdoNetApplicationServiceInvoker>();
            return invoker.Invoke<TService, TResult>(new AdoNetApplicationServiceContext(payload, user, Transaction));
        }
    }

    public abstract class AdoNetApplicationController : AdoNetApplicationController<User> { }
}