using System;

namespace Lumen
{
    public class PipelineContext<TServiceBase>
        where TServiceBase : ApplicationServiceBase
    {
        public PipelineContext(TServiceBase service)
        {
            Service = Ensure.NotNull(service, "service");
        }

        public TServiceBase Service { get; private set; }
        public Type ServiceType { get { return Service.GetType(); } }
    }

    public class PipelineContext : PipelineContext<ApplicationServiceBase>
    {
        public PipelineContext(ApplicationServiceBase service) : base(service) { }
    }
}