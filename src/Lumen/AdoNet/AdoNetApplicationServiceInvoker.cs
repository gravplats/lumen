namespace Lumen.AdoNet
{
    public class AdoNetApplicationServiceInvoker : ApplicationServiceInvoker<AdoNetApplicationServiceContext>
    {
        public AdoNetApplicationServiceInvoker(AdoNetApplicationServiceFactory serviceFactory, AdoNetApplicationServicePipelineFactory pipelineFactory)
            : base(serviceFactory, pipelineFactory) { }
    }
}