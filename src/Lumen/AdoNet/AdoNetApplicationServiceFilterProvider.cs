using Ninject;

namespace Lumen.AdoNet
{
    public class AdoNetApplicationServiceFilterProvider : ApplicationServiceFilterProvider<AdoNetApplicationServiceContext>
    {
        public AdoNetApplicationServiceFilterProvider(IKernel kernel) : base(kernel) { }
    }
}