namespace Lumen.AdoNet
{
    public class AdoNetApplicationServicePipelineFactory : ApplicationServicePipelineFactory<AdoNetApplicationServiceContext>
    {
        public AdoNetApplicationServicePipelineFactory() { }

        public AdoNetApplicationServicePipelineFactory(AdoNetApplicationServiceFilterProvider provider) : base(provider) { }
    }
}