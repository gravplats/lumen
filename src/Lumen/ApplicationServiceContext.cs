using Lumen.Security;

namespace Lumen
{
    public class ApplicationServiceContext
    {
        public ApplicationServiceContext(dynamic payload, User user)
        {
            Payload = Ensure.NotNull(payload, "payload");
            User = Ensure.NotNull(user, "user");
        }

        public dynamic Payload { get; private set; }
        public User User { get; set; }
    }
}