using System.Collections.Generic;
using System.Linq;
using Ninject;

namespace Lumen
{
    public class ApplicationServiceFilterProvider<TContext>
        where TContext : class
    {
        private readonly IKernel kernel;

        public ApplicationServiceFilterProvider(IKernel kernel)
        {
            this.kernel = Ensure.NotNull(kernel, "kernel");
        }

        public IList<ApplicationServiceFilter<TContext>> GetFilters()
        {
            return kernel.GetAll<ApplicationServiceFilter<TContext>>().ToList();
        }
    }

    public class ApplicationServiceFilterProvider : ApplicationServiceFilterProvider<ApplicationServiceContext>
    {
        public ApplicationServiceFilterProvider(IKernel kernel) : base(kernel) { }
    }
}