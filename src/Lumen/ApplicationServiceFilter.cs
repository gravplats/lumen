namespace Lumen
{
    public abstract class ApplicationServiceFilter<TContext>
        where TContext : class
    {
        public abstract void Process<TService, TResult>(PipelineContext<TService, TResult> pipelineContext, TContext context)
            where TService : ApplicationService<TResult>;
    }
}