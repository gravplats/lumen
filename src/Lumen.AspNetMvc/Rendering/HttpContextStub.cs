using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Web.Instrumentation;

namespace Lumen.AspNetMvc.Rendering
{
    internal class HttpContextStub : HttpContextBase
    {
        private readonly HttpRequestBase request;
        private readonly HttpResponseBase response;

        public HttpContextStub(HttpResponse httpResponse, bool isMobileDevice)
        {
            request = new HttpRequestStub(new HttpRequest("", "http://www.example.com", "")) { IsMobileDevice = isMobileDevice };
            response = new HttpResponseWrapper(httpResponse);
        }

        public override Cache Cache
        {
            // Not sure this is the best idea, I rather ignore caching altogther.
            get { return HttpRuntime.Cache; }
        }

        public override PageInstrumentationService PageInstrumentation
        {
            get { return new PageInstrumentationService(); }
        }

        public override HttpRequestBase Request
        {
            get { return request; }
        }

        public override HttpResponseBase Response
        {
            get { return response; }
        }

        private IDictionary items;
        public override IDictionary Items
        {
            get { return items ?? (items = new Hashtable()); }
        }
    }
}