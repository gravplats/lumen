using System.Web.Mvc;
using Lumen.AspNetMvc;
using Ninject;

namespace Lumen.Tests
{
    public class CustomPipelineContextTests
    {
        public interface ICustomApplicationService : IApplicationService
        {
            bool IsAuthorized();
        }

        public abstract class CustomApplicationService : ApplicationService, ICustomApplicationService
        {
            public abstract bool IsAuthorized();
        }

        public abstract class CustomApplicationService<TResult> : ApplicationService<TResult>, ICustomApplicationService
        {
            public abstract bool IsAuthorized();
        }


        public class CustomPipelineContext : PipelineContext<ICustomApplicationService>
        {
            public CustomPipelineContext(ICustomApplicationService service) : base(service) { }
        }

        public class CustomApplicationServiceFilter : ApplicationServiceFilter<ApplicationServiceContext, CustomPipelineContext>
        {
            public override void Process(ApplicationServiceContext context, CustomPipelineContext pipelineContext)
            {
                if (pipelineContext.Service.IsAuthorized())
                {
                    return;
                }

                throw new ApplicationServiceAuthorizationException();
            }
        }

        public class MyService : CustomApplicationService
        {
            public class Payload { }

            public override bool IsAuthorized()
            {
                return true;
            }

            protected override void ExecuteCore() { }
        }


        public class MyController : ApplicationController
        {
            protected override TResult InvokeService<TService, TPayload, TResult>(TPayload payload)
            {
                var invoker = DependencyResolver.Current.GetService<ApplicationServiceInvoker<ApplicationServiceContext, CustomPipelineContext>>();
                return invoker.Invoke<TService, TResult>(new ApplicationServiceContext(payload));
            }

            public ActionResult My()
            {
                return Invoke<MyService, MyService.Payload>();
            }
        }

        protected IKernel GetKernel()
        {
            var kernel = new StandardKernel();
            kernel.BindApplicationServicePipeline<ApplicationServiceContext, CustomPipelineContext>()
                  .WithFilter<CustomApplicationServiceFilter>();

            return kernel;
        }
    }
}