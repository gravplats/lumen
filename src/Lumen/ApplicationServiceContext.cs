using Lumen.Security;

namespace Lumen
{
    public class ApplicationServiceContext
    {
        public ApplicationServiceContext(dynamic payload, IUser user)
        {
            Payload = Ensure.NotNull(payload, "payload");
            // The current user will be null if not authenticated.
            User = user;
        }

        public dynamic Payload { get; private set; }
        public IUser User { get; private set; }
    }
}