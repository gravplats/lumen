using System;
using System.Linq;

namespace Lumen
{
    public class ApplicationServicePipelineFactory<TContext>
        where TContext : class
    {
        private readonly ApplicationServiceFilterProvider<TContext> provider;

        public ApplicationServicePipelineFactory() { }

        public ApplicationServicePipelineFactory(ApplicationServiceFilterProvider<TContext> provider)
        {
            this.provider = Ensure.NotNull(provider, "provider");
        }

        public Func<PipelineContext, TContext, TResult> Create<TResult>(Func<PipelineContext, TContext, TResult> sink)
        {
            Ensure.NotNull(sink, "sink");

            if (provider == null)
            {
                return sink;
            }

            var filters = provider.GetFilters();

            var filter = filters.FirstOrDefault();
            if (filter == null)
            {
                return sink;
            }

            ApplicationServiceFilter<TContext> current = filter, last = filter;
            for (int index = 0; index < filters.Count - 1; index++)
            {
                last = filters[index + 1];

                Func<PipelineContext, TContext, TResult> func = last.Process<TResult>;
                current.Register(func);

                current = last;
            }

            last.Register(sink);

            return filter.Process<TResult>;
        }
    }
}