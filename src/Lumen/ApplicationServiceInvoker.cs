namespace Lumen
{
    public class ApplicationServiceInvoker<TContext>
        where TContext : class
    {
        private readonly ApplicationServiceFactory<TContext> serviceFactory;
        private readonly ApplicationServiceFilterProvider<TContext> filterProvider;

        public ApplicationServiceInvoker(ApplicationServiceFactory<TContext> serviceFactory, ApplicationServiceFilterProvider<TContext> filterProvider)
        {
            this.serviceFactory = Ensure.NotNull(serviceFactory, "serviceFactory");
            this.filterProvider = Ensure.NotNull(filterProvider, "filterProvider");
        }

        public TResult Invoke<TService, TResult>(TContext context)
            where TService : ApplicationService<TResult>
        {
            Ensure.NotNull(context, "context");

            var pipelineContext = new PipelineContext(typeof(TService));

            var filters = filterProvider.GetFilters();
            foreach (var filter in filters)
            {
                filter.Process(pipelineContext, context);
            }

            return serviceFactory.Create<TService, TResult>(context).Execute();
        }
    }

    public class ApplicationServiceInvoker : ApplicationServiceInvoker<ApplicationServiceContext>
    {
        public ApplicationServiceInvoker(ApplicationServiceFactory serviceFactory, ApplicationServiceFilterProvider filterProvider)
            : base(serviceFactory, filterProvider)
        {
        }
    }
}