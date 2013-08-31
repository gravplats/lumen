using System.Web;

namespace Lumen.AspNetMvc.Rendering
{
    internal class HttpRequestStub : HttpRequestWrapper
    {
        public HttpRequestStub(HttpRequest httpRequest) : base(httpRequest) { }

        public bool IsMobileDevice { get; set; }

        // If we don't override something breaks in ASP.NET MVC 4 due to display modes.
        public override HttpBrowserCapabilitiesBase Browser
        {
            get
            {
                var capabilities = new HttpBrowserCapabilitiesStub(IsMobileDevice);
                return new HttpBrowserCapabilitiesWrapper(capabilities);
            }
        }
    }
}