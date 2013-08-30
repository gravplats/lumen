using Ninject;
using Ninject.Parameters;

namespace Lumen.AspNetMvc.Raven
{
    public class RavenApplicationServiceFactory : ApplicationServiceFactory<RavenApplicationServiceContext>
    {
        private readonly IKernel kernel;

        public RavenApplicationServiceFactory(IKernel kernel)
        {
            this.kernel = Ensure.NotNull(kernel, "kernel");
        }

        public override TService Create<TService, TResult>(RavenApplicationServiceContext context)
        {
            return kernel.Get<TService>(
                new ConstructorArgument("payload", context.Payload),
                new ConstructorArgument("currentUser", context.User),
                new ConstructorArgument("session", context.Session)
            );
        }
    }
}