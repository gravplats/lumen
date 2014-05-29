using System.Web.Mvc;
using Lumen.AspNetMvc;

namespace Lumen.Tests
{
    public class ApplicationControllerTests
    {
        public class Payload { }

        public class Result { }

        public class TestApplicationController : ApplicationController
        {
            public class ApplicationServiceWithoutPayloadAndResult : ApplicationService
            {
                protected override void ExecuteCore() { }
            }

            public ActionResult WithoutPayloadAndResult()
            {
                return Invoke<ApplicationServiceWithoutPayloadAndResult>((p, r) => HttpOk());
            }


            public class ApplicationServiceWithPayload : ApplicationService
            {
                protected override void ExecuteCore() { }
            }

            public ActionResult WithPayload()
            {
                return Invoke<ApplicationServiceWithPayload, Payload>((p, r) => HttpOk());
            }


            public class ApplicationServiceWithResult : ApplicationService<Result>
            {
                protected override Result ExecuteCore()
                {
                    return new Result();
                }
            }

            public ActionResult WithResult()
            {
                return InvokeWithResult<ApplicationServiceWithResult, Result>((p, r) => HttpOk());
            }


            public class ApplicationServiceWithPayloadAndResult : ApplicationService<Result>
            {
                protected override Result ExecuteCore()
                {
                    return new Result();
                }
            }

            public ActionResult WithPayloadAndResult()
            {
                return Invoke<ApplicationServiceWithPayloadAndResult, Payload, Result>((p, r) => HttpOk());
            }
        }
    }
}