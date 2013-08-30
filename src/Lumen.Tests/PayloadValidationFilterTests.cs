using FluentValidation;
using Lumen.Security;
using NUnit.Framework;
using Ninject;

namespace Lumen.Tests
{
    [TestFixture]
    public class PayloadValidationFilterTests
    {
        public class Payload
        {
            public string Text { get; set; }
        }

        public class PayloadValidator : AbstractValidator<Payload>
        {
            public PayloadValidator()
            {
                RuleFor(x => x.Text).NotEmpty();
            }
        }

        [Test]
        public void Can_throw_on_non_valid_payload()
        {
            var kernel = new StandardKernel();
            kernel.Bind<PayloadValidator>().ToSelf().InSingletonScope();
            kernel.Bind<IValidator<Payload>>().To<PayloadValidator>().InSingletonScope();
            kernel.Bind<PayloadValidationFilter>().ToSelf().InSingletonScope();

            var context = new ApplicationServiceContext(new Payload(), new User());

            var filter = kernel.Get<PayloadValidationFilter>();
            filter.Register<object>(ctx => null);

            Assert.Throws<PayloadValidationException>(() => filter.Process<object>(context));
        }
    }
}