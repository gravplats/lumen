namespace Lumen
{
    public class ApplicationServiceInvoker<TContext>
        where TContext : class
    {
        private readonly ApplicationServiceFactory<TContext> serviceFactory;
        private readonly ApplicationServicePipelineFactory<TContext> pipelineFactory;

        public ApplicationServiceInvoker(ApplicationServiceFactory<TContext> serviceFactory, ApplicationServicePipelineFactory<TContext> pipelineFactory)
        {
            this.serviceFactory = Ensure.NotNull(serviceFactory, "serviceFactory");
            this.pipelineFactory = Ensure.NotNull(pipelineFactory, "pipelineFactory");
        }

        public TResult Invoke<TService, TResult>(TContext context)
            where TService : ApplicationService<TResult>
        {
            Ensure.NotNull(context, "context");

            var pipelineContext = new PipelineContext(typeof(TService));
            var pipeline = pipelineFactory.Create(ExecuteService<TService, TResult>);

            return pipeline(pipelineContext, context);
        }

        private TResult ExecuteService<TService, TResult>(PipelineContext pipelineContext, TContext context)
            where TService : ApplicationService<TResult>
        {
            return serviceFactory.Create<TService, TResult>(context).Execute();
        }
    }

    public class ApplicationServiceInvoker : ApplicationServiceInvoker<ApplicationServiceContext>
    {
        public ApplicationServiceInvoker(ApplicationServiceFactory serviceFactory, ApplicationServicePipelineFactory pipelineFactory)
            : base(serviceFactory, pipelineFactory)
        {
        }
    }
}