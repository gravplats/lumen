namespace Lumen
{
    public abstract class ApplicationServiceFilter<TContext, TPipelineContext>
        where TContext : class
    {
        public abstract void Process(TContext context, TPipelineContext pipelineContext);
    }
}