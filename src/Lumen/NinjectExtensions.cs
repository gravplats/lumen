using Ninject.Syntax;

namespace Lumen
{
    public static class NinjectExtensions
    {
        public class PipelineBindingContinuation<TContext, TPipelineContext>
            where TContext : class
        {
            private readonly IBindingRoot root;

            public PipelineBindingContinuation(IBindingRoot root)
            {
                this.root = Ensure.NotNull(root, "root");
            }

            public PipelineBindingContinuation<TContext, TPipelineContext> WithFilter<TApplicationServiceFilter>()
                where TApplicationServiceFilter : ApplicationServiceFilter<TContext, TPipelineContext>
            {
                root.Bind<ApplicationServiceFilter<TContext, TPipelineContext>>().To<TApplicationServiceFilter>();
                return this;
            }
        }

        public static PipelineBindingContinuation<ApplicationServiceContext, PipelineContext> BindApplicationServicePipeline(this IBindingRoot root)
        {
            root.Bind<ApplicationServiceInvoker>().ToSelf().InSingletonScope();
            root.Bind<ApplicationServiceFactory>().ToSelf().InSingletonScope();
            root.Bind<ApplicationServiceFilterProvider>().ToSelf().InSingletonScope();

            return new PipelineBindingContinuation<ApplicationServiceContext, PipelineContext>(root);
        }

        public static PipelineBindingContinuation<TContext, TPipelineContext> BindApplicationServicePipeline<TContext, TPipelineContext>(this IBindingRoot root)
            where TContext : class
        {
            root.Bind<ApplicationServiceInvoker<TContext, TPipelineContext>>().ToSelf().InSingletonScope();
            root.Bind<ApplicationServiceFactory<TContext>>().ToSelf().InSingletonScope();
            root.Bind<ApplicationServiceFilterProvider<TContext, TPipelineContext>>().ToSelf().InSingletonScope();

            return new PipelineBindingContinuation<TContext, TPipelineContext>(root);
        }
    }
}