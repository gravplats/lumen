namespace Lumen
{
    public class ApplicationServiceContext : IPayload
    {
        public ApplicationServiceContext(dynamic payload)
        {
            Payload = Ensure.NotNull(payload, "payload");
        }

        public dynamic Payload { get; private set; }
    }
}