using System.Collections.Generic;
using System.Linq;
using Ninject;

namespace Lumen
{
    public class ApplicationServiceFilterProvider<TContext, TPipelineContext>
        where TContext : class
    {
        private readonly IKernel kernel;

        public ApplicationServiceFilterProvider(IKernel kernel)
        {
            this.kernel = Ensure.NotNull(kernel, "kernel");
        }

        public IList<ApplicationServiceFilter<TContext, TPipelineContext>> GetFilters()
        {
            return kernel.GetAll<ApplicationServiceFilter<TContext, TPipelineContext>>().ToList();
        }
    }

    public class ApplicationServiceFilterProvider : ApplicationServiceFilterProvider<ApplicationServiceContext, PipelineContext>
    {
        public ApplicationServiceFilterProvider(IKernel kernel) : base(kernel) { }
    }
}