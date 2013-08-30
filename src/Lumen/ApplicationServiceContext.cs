using Lumen.Security;

namespace Lumen
{
    public class ApplicationServiceContext
    {
        public ApplicationServiceContext(dynamic payload, IUser user)
        {
            Payload = Ensure.NotNull(payload, "payload");
            User = Ensure.NotNull(user, "user");
        }

        public dynamic Payload { get; private set; }
        public IUser User { get; private set; }
    }
}