namespace Lumen
{
    public class PayloadValidationFilter<TContext, TPipelineContext> : ApplicationServiceFilter<TContext, TPipelineContext>
        where TPipelineContext : class
        where TContext : class, IApplicationServiceContext
    {
        private readonly PayloadValidator validator;

        public PayloadValidationFilter(PayloadValidator validator)
        {
            this.validator = Ensure.NotNull(validator, "validator");
        }

        public override void Process(TContext context, TPipelineContext pipelineContext)
        {
            validator.Validate(context.Payload);
        }
    }
}