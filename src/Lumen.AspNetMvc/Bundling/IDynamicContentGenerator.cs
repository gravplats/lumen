using System.Collections.Generic;
using System.Web;

namespace Lumen.AspNetMvc.Bundling
{
    public interface IDynamicContentGenerator
    {
        string Filter { get; }

        IEnumerable<string> WatchedVirtualPaths { get; }
        
        string GenerateContent(HttpContextBase httpContext);
    }
}