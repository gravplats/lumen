using Ninject;
using Ninject.Parameters;

namespace Lumen
{
    public abstract class ApplicationServiceFactory<TContext>
        where TContext : class
    {
        public abstract TService Create<TService, TResult>(TContext context)
            where TService : ApplicationService<TResult>;
    }

    public class ApplicationServiceFactory : ApplicationServiceFactory<ApplicationServiceContext>
    {
        private readonly IKernel kernel;

        public ApplicationServiceFactory(IKernel kernel)
        {
            this.kernel = Ensure.NotNull(kernel, "kernel");
        }

        public override TService Create<TService, TResult>(ApplicationServiceContext context)
        {
            return kernel.Get<TService>(
                new ConstructorArgument("payload", context.Payload),
                new ConstructorArgument("currentUser", context.User)
            );
        }
    }
}