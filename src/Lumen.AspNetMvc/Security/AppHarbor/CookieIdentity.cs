using System;
using System.Security.Principal;

namespace Lumen.AspNetMvc.Security.AppHarbor
{
    // Ideally, we would inherit from GenericIdentity and would not have MarshalByRefObject here. However, Cassini has a long
    // time bug that makes it throw a SerializationException at runtime. Inheriting from MarshalByRefObject works around that bug.
    [Serializable]
    public class CookieIdentity : MarshalByRefObject, IIdentity
    {
        private readonly AuthenticationCookie cookie;

        public CookieIdentity(AuthenticationCookie cookie)
        {
            this.cookie = Ensure.NotNull(cookie, "cookie");
        }

        public Guid AuthenticationToken
        {
            get { return cookie.Id; }
        }

        public string AuthenticationType
        {
            get { return "cookie"; }
        }

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        public string Name
        {
            get { return cookie.Name; }
        }
    }
}
