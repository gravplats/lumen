using System;
using NUnit.Framework;
using Ninject;
using Ninject.Parameters;

namespace Lumen.Tests
{
    [TestFixture]
    public class ApplicationServiceInvokerTests
    {
        public class TestContext { }

        public class TestFilter : ApplicationServiceFilter<TestContext>
        {
            protected override TResult ProcessCore<TResult>(TestContext context, Func<TestContext, TResult> next)
            {
                return next(context);
            }
        }

        public class TestApplicationService : ApplicationService
        {
            protected override void ExecuteCore() { }
        }

        public class NinjectApplicationServiceFactory : ApplicationServiceFactory<TestContext>
        {
            private readonly IKernel kernel;

            public NinjectApplicationServiceFactory(IKernel kernel)
            {
                this.kernel = kernel;
            }

            public override TService Create<TService, TResult>(TestContext context)
            {
                return kernel.Get<TService>(new ConstructorArgument("context", context));
            }
        }

        [Test]
        public void Can_invoke_service_without_filters()
        {
            var kernel = new StandardKernel();
            kernel.Bind<ApplicationServiceInvoker<TestContext>>().ToSelf().InSingletonScope();
            kernel.Bind<ApplicationServiceFactory<TestContext>>().To<NinjectApplicationServiceFactory>().InSingletonScope();
            kernel.Bind<ApplicationServicePipelineFactory<TestContext>>().ToSelf().InSingletonScope();

            var context = new TestContext();

            var invoker = kernel.Get<ApplicationServiceInvoker<TestContext>>();
            var result = invoker.Invoke<TestApplicationService, object>(context);

            result.ShouldBeNull();
        }

        [Test]
        public void Can_invoke_service_with_filters()
        {
            var kernel = new StandardKernel();
            kernel.Bind<ApplicationServiceInvoker<TestContext>>().ToSelf().InSingletonScope();
            kernel.Bind<ApplicationServiceFactory<TestContext>>().To<NinjectApplicationServiceFactory>().InSingletonScope();
            kernel.Bind<ApplicationServicePipelineFactory<TestContext>>().ToSelf().InSingletonScope();
            kernel.Bind<ApplicationServiceFilterProvider<TestContext>>().ToSelf().InSingletonScope();
            kernel.Bind<ApplicationServiceFilter<TestContext>>().To<TestFilter>();

            var context = new TestContext();

            var invoker = kernel.Get<ApplicationServiceInvoker<TestContext>>();
            var result = invoker.Invoke<TestApplicationService, object>(context);

            result.ShouldBeNull();
        }
    }
}