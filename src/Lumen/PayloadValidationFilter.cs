using System;

namespace Lumen
{
    public class PayloadValidationFilter<TContext> : ApplicationServiceFilter<TContext>
        where TContext : class, IPayload
    {
        private readonly PayloadValidator validator;

        public PayloadValidationFilter(PayloadValidator validator)
        {
            this.validator = Ensure.NotNull(validator, "validator");
        }

        protected override TResult ProcessCore<TResult>(PipelineContext pipelineContext, TContext context, Func<PipelineContext, TContext, TResult> next)
        {
            validator.Validate(context.Payload);
            return next(pipelineContext, context);
        }
    }
}