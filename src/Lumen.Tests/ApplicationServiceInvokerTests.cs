using NUnit.Framework;
using Ninject;

namespace Lumen.Tests
{
    [TestFixture]
    public class ApplicationServiceInvokerTests
    {
        public class EchoPayloadResult
        {
            public int Value { get; set; }
        }

        public class ModifyPayloadFilter : ApplicationServiceFilter<ApplicationServiceContext, PipelineContext>
        {
            public override void Process(ApplicationServiceContext context, PipelineContext pipelineContext)
            {
                context.Payload.Value = 2;
            }
        }

        public class EchoPayloadValueService : ApplicationService<EchoPayloadResult>
        {
            public class Payload
            {
                public int Value { get; set; }
            }

            private readonly Payload payload;

            public EchoPayloadValueService(Payload payload)
            {
                this.payload = payload;
            }

            protected override EchoPayloadResult ExecuteCore()
            {
                return new EchoPayloadResult
                {
                    Value = payload.Value
                };
            }
        }

        [Test]
        public void Can_invoke_service_without_filters()
        {
            // Assert.
            var kernel = new StandardKernel();
            kernel.BindApplicationServicePipeline();

            var payload = new EchoPayloadValueService.Payload { Value = 1 };
            var context = new ApplicationServiceContext(payload);

            // Act.
            var result = kernel.Get<ApplicationServiceInvoker>().Invoke<EchoPayloadValueService, EchoPayloadResult>(context);

            // Assert.
            result.Value.ShouldBe(1);
        }

        [Test]
        public void Can_invoke_service_with_filters()
        {
            // Assert.
            var kernel = new StandardKernel();
            kernel.BindApplicationServicePipeline()
                  .WithFilter<ModifyPayloadFilter>();

            var payload = new EchoPayloadValueService.Payload { Value = 1 };
            var context = new ApplicationServiceContext(payload);

            // Act.
            var result = kernel.Get<ApplicationServiceInvoker>().Invoke<EchoPayloadValueService, EchoPayloadResult>(context);

            // Assert.
            result.Value.ShouldBe(2);
        }
    }
}