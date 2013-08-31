using System.Data;
using Lumen.Security;

namespace Lumen.AdoNet
{
    public class AdoNetApplicationServiceContext : IPayload
    {
        public AdoNetApplicationServiceContext(dynamic payload, IUser user, IDbTransaction transaction)
        {
            Payload = Ensure.NotNull(payload, "payload");
            // The current user will be null if not authenticated.
            User = user;
            Transaction = Ensure.NotNull(transaction, "transaction");
        }

        public dynamic Payload { get; private set; }
        public IDbTransaction Transaction { get; private set; }
        public IUser User { get; private set; }
    }
}