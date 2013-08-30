using System.Security.Principal;

namespace Lumen.Security
{
    public class ApplicationIdentity<TUser> : IIdentity
        where TUser : class, IUser
    {
        public ApplicationIdentity(TUser user = null)
        {
            User = user;
        }

        public TUser User { get; private set; }

        public string Name
        {
            get { return User == null ? null : User.Username; }
        }

        public string AuthenticationType
        {
            get { return "Lumen"; }
        }

        public bool IsAuthenticated
        {
            get { return User != null; }
        }
    }
}