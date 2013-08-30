using System.Linq;
using FluentValidation;
using Ninject;

namespace Lumen
{
    public class PayloadValidator
    {
        private readonly IKernel kernel;

        public PayloadValidator(IKernel kernel)
        {
            this.kernel = Ensure.NotNull(kernel, "kernel");
        }

        public void Validate<TPayload>(TPayload payload)
        {
            var validator = kernel.TryGet<IValidator<TPayload>>();
            if (validator == null)
            {
                return;
            }

            var validation = validator.Validate(payload);
            if (validation.IsValid)
            {
                return;
            }

            var fields = validation.Errors.ToDictionary(x => x.PropertyName, x => x.ErrorMessage);
            throw new PayloadValidationException(fields);
        }
    }
}