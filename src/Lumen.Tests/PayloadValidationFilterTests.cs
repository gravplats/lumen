using FluentValidation;
using NUnit.Framework;
using Ninject;

namespace Lumen.Tests
{
    [TestFixture]
    public class PayloadValidationFilterTests
    {
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
            // Arrange.
            var kernel = new StandardKernel();
            kernel.Bind<PayloadValidator>().ToSelf().InSingletonScope();
            kernel.Bind<IValidator<TestApplicationService.TestPayload>>().To<TestApplicationService.TestPayloadValidator>().InSingletonScope();
            kernel.Bind<PayloadValidationFilter<ApplicationServiceContext, PipelineContext>>().ToSelf().InSingletonScope();

            var filter = kernel.Get<PayloadValidationFilter<ApplicationServiceContext, PipelineContext>>();
            var service = new TestApplicationService();
            var context = new ApplicationServiceContext(new TestApplicationService.TestPayload());

            // Act & Assert.
            Assert.Throws<PayloadValidationException>(() => filter.Process(context, new PipelineContext(service)));
        }
    }
}