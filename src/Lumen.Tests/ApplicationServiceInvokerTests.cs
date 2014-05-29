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

        public class ModifyPayloadFilter : ApplicationServiceFilter<ApplicationServiceContext>
        {
            public override void Process<TService, TResult>(PipelineContext<TService, TResult> pipelineContext, ApplicationServiceContext context)
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

            public override EchoPayloadResult Execute()
            {
                return new EchoPayloadResult
                {
                    Value = payload.Value
                };
            }
        }

        protected IKernel GetKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<ApplicationServiceInvoker>().ToSelf().InSingletonScope();
            kernel.Bind<ApplicationServiceFactory>().ToSelf().InSingletonScope();
            kernel.Bind<ApplicationServiceFilterProvider>().ToSelf().InSingletonScope();

            return kernel;
        }

        [Test]
        public void Can_invoke_service_without_filters()
        {
            // Assert.
            var kernel = GetKernel();

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
            var kernel = GetKernel();
            kernel.Bind<ApplicationServiceFilter<ApplicationServiceContext>>().To<ModifyPayloadFilter>();

            var payload = new EchoPayloadValueService.Payload { Value = 1 };
            var context = new ApplicationServiceContext(payload);

            // Act.
            var result = kernel.Get<ApplicationServiceInvoker>().Invoke<EchoPayloadValueService, EchoPayloadResult>(context);

            // Assert.
            result.Value.ShouldBe(2);
        }
    }
}