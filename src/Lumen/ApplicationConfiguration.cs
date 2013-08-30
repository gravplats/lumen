using System.Linq;
using System.Reflection;
using Ninject;

namespace Lumen
{
    public abstract class ApplicationConfiguration
    {
        public abstract void Register();

        public static void Execute(IKernel kernel, params Assembly[] assemblies)
        {
            Ensure.NotNull(kernel, "kernel");

            var types = assemblies
                .SelectMany(x => x.GetExportedTypes())
                .Where(x => typeof(ApplicationConfiguration).IsAssignableFrom(x))
                .ToList();

            foreach (var type in types)
            {
                kernel.Bind(type).ToSelf();
            }

            foreach (var type in types.TopoSort())
            {
                var configuration = (ApplicationConfiguration)kernel.Get(type);
                configuration.Register();

                kernel.Unbind(type);
            }
        }
    }
}