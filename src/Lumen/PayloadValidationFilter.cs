using System;

namespace Lumen
{
    public class PayloadValidationFilter : ApplicationServiceFilter<ApplicationServiceContext>
    {
        private readonly PayloadValidator validator;

        public PayloadValidationFilter(PayloadValidator validator)
        {
            this.validator = Ensure.NotNull(validator, "validator");
        }

        protected override TResult ProcessCore<TResult>(ApplicationServiceContext context, Func<ApplicationServiceContext, TResult> next)
        {
            validator.Validate(context.Payload);
            return next(context);
        }
    }
}