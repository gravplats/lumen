using System.Web;

namespace Lumen.AspNetMvc.Rendering
{
    internal class HttpBrowserCapabilitiesStub : HttpBrowserCapabilities
    {
        private readonly bool isMobileDevice;

        public HttpBrowserCapabilitiesStub(bool isMobileDevice)
        {
            this.isMobileDevice = isMobileDevice;
        }

        public override bool IsMobileDevice
        {
            get { return isMobileDevice; }
        }
    }
}