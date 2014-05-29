using System;

namespace Lumen
{
    public class PipelineContext<TService, TResult>
        where TService : ApplicationService<TResult>
    {
        public PipelineContext(TService service)
        {
            Service = Ensure.NotNull(service, "service");
        }

        public TService Service { get; private set; }
        public Type ServiceType { get { return Service.GetType(); } }
    }
}