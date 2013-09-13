using System;

namespace Lumen
{
    public abstract class ApplicationServiceFilter<TContext>
        where TContext : class
    {
        private object next;

        public TResult Process<TResult>(PipelineContext pipelineContext, TContext context)
        {
            return ProcessCore(pipelineContext, context, (Func<PipelineContext, TContext, TResult>)next);
        }

        protected abstract TResult ProcessCore<TResult>(PipelineContext pipelineContext, TContext context, Func<PipelineContext, TContext, TResult> next);

        public virtual void Register<TResult>(Func<PipelineContext, TContext, TResult> process)
        {
            next = Ensure.NotNull(process, "process");
        }
    }
}