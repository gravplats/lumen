namespace Lumen
{
    public class ApplicationServiceContext : IApplicationServiceContext
    {
        public ApplicationServiceContext(dynamic payload)
        {
            Payload = Ensure.NotNull(payload, "payload");
        }

        public dynamic Payload { get; private set; }
    }
}