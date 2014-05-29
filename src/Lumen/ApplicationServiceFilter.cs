namespace Lumen
{
    public abstract class ApplicationServiceFilter<TContext>
        where TContext : class
    {
        public abstract void Process(PipelineContext pipelineContext, TContext context);
    }
}