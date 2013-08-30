using System;

namespace Lumen
{
    public abstract class ApplicationServiceFilter<TContext>
        where TContext : class
    {
        private object next;

        public TResult Process<TResult>(TContext context)
        {
            return ProcessCore(context, (Func<TContext, TResult>)next);
        }

        protected abstract TResult ProcessCore<TResult>(TContext context, Func<TContext, TResult> next);

        public virtual void Register<TResult>(Func<TContext, TResult> process)
        {
            next = Ensure.NotNull(process, "process");
        }
    }
}