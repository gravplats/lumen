using System;
using System.Security.Principal;

namespace Lumen.Security
{
    public class ApplicationPrincipal<TUser> : IPrincipal
        where TUser : class, IUser
    {
        private ApplicationPrincipal(ApplicationIdentity<TUser> identity)
        {
            Identity = identity;
        }

        public ApplicationIdentity<TUser> Identity { get; private set; }

        IIdentity IPrincipal.Identity { get { return Identity; } }

        bool IPrincipal.IsInRole(string role)
        {
            throw new NotSupportedException();
        }

        public static ApplicationPrincipal<TUser> Create(TUser user = null)
        {
            var identity = new ApplicationIdentity<TUser>(user);
            return new ApplicationPrincipal<TUser>(identity);
        }
    }
}