using System.Collections.Generic;
using System.Web;

namespace Lumen.AspNetMvc.Bundling
{
    public interface IDynamicContentGenerator
    {
        IEnumerable<string> WatchedVirtualPaths { get; }
        
        string GenerateContent(HttpContextBase httpContext);
    }
}