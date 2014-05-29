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

        public override void Process(PipelineContext pipelineContext, TContext context)
        {
            validator.Validate(context.Payload);
        }
    }
}