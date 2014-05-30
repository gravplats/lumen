using System;

namespace Lumen
{
    public class PipelineContext<TServiceBase>
        where TServiceBase : class, IApplicationService
    {
        public PipelineContext(TServiceBase service)
        {
            Service = Ensure.NotNull(service, "service");
        }

        public TServiceBase Service { get; private set; }
        public Type ServiceType { get { return Service.GetType(); } }
    }

    public class PipelineContext : PipelineContext<IApplicationService>
    {
        public PipelineContext(IApplicationService service) : base(service) { }
    }
}