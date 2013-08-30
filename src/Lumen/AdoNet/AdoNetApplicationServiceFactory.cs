using Ninject;
using Ninject.Parameters;

namespace Lumen.AdoNet
{
    public class AdoNetApplicationServiceFactory : ApplicationServiceFactory<AdoNetApplicationServiceContext>
    {
        private readonly IKernel kernel;

        public AdoNetApplicationServiceFactory(IKernel kernel)
        {
            this.kernel = Ensure.NotNull(kernel, "kernel");
        }

        public override TService Create<TService, TResult>(AdoNetApplicationServiceContext context)
        {
            return kernel.Get<TService>(
                new ConstructorArgument("payload", context.Payload),
                new ConstructorArgument("currentUser", context.User),
                new ConstructorArgument("connection", context.Transaction.Connection)
            );
        }
    }
}