using FluentValidation;
using NUnit.Framework;
using Ninject;

namespace Lumen.Tests
{
    [TestFixture]
    public class PayloadValidationFilterTests
    {
        public class TestContext : IPayload
        {
            public TestContext(dynamic payload)
            {
                Payload = payload;
            }

            public dynamic Payload { get; private set; }
        }

        public class TestApplicationService : ApplicationService
        {
            public class TestPayload
            {
                public string Text { get; set; }
            }

            public class TestPayloadValidator : AbstractValidator<TestPayload>
            {
                public TestPayloadValidator()
                {
                    RuleFor(x => x.Text).NotEmpty();
                }
            }

            protected override void ExecuteCore() { }
        }

        [Test]
        public void Can_throw_on_non_valid_payload()
        {
            var kernel = new StandardKernel();
            kernel.Bind<PayloadValidator>().ToSelf().InSingletonScope();
            kernel.Bind<IValidator<TestApplicationService.TestPayload>>().To<TestApplicationService.TestPayloadValidator>().InSingletonScope();
            kernel.Bind<PayloadValidationFilter<TestContext>>().ToSelf().InSingletonScope();

            var context = new TestContext(new TestApplicationService.TestPayload());

            var filter = kernel.Get<PayloadValidationFilter<TestContext>>();
            var service = new TestApplicationService();

            Assert.Throws<PayloadValidationException>(() => filter.Process(new PipelineContext<TestApplicationService, object>(service), context));
        }
    }
}