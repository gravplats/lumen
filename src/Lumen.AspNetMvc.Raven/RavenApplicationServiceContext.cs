using Lumen.Security;
using Raven.Client;

namespace Lumen.AspNetMvc.Raven
{
    public class RavenApplicationServiceContext
    {
        public RavenApplicationServiceContext(dynamic payload, IUser user, IDocumentSession session)
        {
            Payload = Ensure.NotNull(payload, "payload");
            User = Ensure.NotNull(user, "user");
            Session = Ensure.NotNull(session, "session");
        }

        public dynamic Payload { get; private set; }
        public IDocumentSession Session { get; private set; }
        public IUser User { get; private set; }
    }
}