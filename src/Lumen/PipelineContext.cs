using System;

namespace Lumen
{
    public class PipelineContext
    {
        public PipelineContext(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public Type ServiceType { get; private set; }
    }
}