namespace Lumen
{
    public abstract class ApplicationServiceInvoker<TContext, TPipelineContext>
        where TContext : class
    {
        private readonly ApplicationServiceFactory<TContext> serviceFactory;
        private readonly ApplicationServiceFilterProvider<TContext, TPipelineContext> filterProvider;

        protected ApplicationServiceInvoker(ApplicationServiceFactory<TContext> serviceFactory, ApplicationServiceFilterProvider<TContext, TPipelineContext> filterProvider)
        {
            this.serviceFactory = Ensure.NotNull(serviceFactory, "serviceFactory");
            this.filterProvider = Ensure.NotNull(filterProvider, "filterProvider");
        }

        public TResult Invoke<TService, TResult>(TContext context)
            where TService : IApplicationService
        {
            Ensure.NotNull(context, "context");

            var service = serviceFactory.Create<TService>(context);
            var pipelineContext = CreatePipelineContext(service);

            var filters = filterProvider.GetFilters();
            foreach (var filter in filters)
            {
                filter.Process(context, pipelineContext);
            }

            return (TResult)service.Execute();
        }

        protected abstract TPipelineContext CreatePipelineContext<TService>(TService service)
            where TService : IApplicationService;
    }

    public class ApplicationServiceInvoker : ApplicationServiceInvoker<ApplicationServiceContext, PipelineContext>
    {
        public ApplicationServiceInvoker(ApplicationServiceFactory serviceFactory, ApplicationServiceFilterProvider filterProvider)
            : base(serviceFactory, filterProvider)
        {
        }

        protected override PipelineContext CreatePipelineContext<TService>(TService service)
        {
            return new PipelineContext(service);
        }
    }
}