using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Lumen.AspNetMvc.Bundling.Mustache
{
    public class TemplateFinder
    {
        private readonly string templateTopVirtualPath;
        private readonly string searchPattern;

        public TemplateFinder(string templateTopVirtualPath, string searchPattern)
        {
            this.templateTopVirtualPath = Ensure.NotNullOrEmpty(templateTopVirtualPath, "templateTopVirtualPath");
            this.searchPattern = Ensure.NotNullOrEmpty(searchPattern, "searchPattern");
        }

        public IEnumerable<Template> GetVirtualPaths(HttpContextBase httpContext)
        {
            string physicalPath = httpContext.Server.MapPath(templateTopVirtualPath);
            var directory = new DirectoryInfo(physicalPath);

            string physicalApplicationPath = httpContext.Request.PhysicalApplicationPath;
            if (string.IsNullOrEmpty(physicalApplicationPath))
            {
                throw new InvalidOperationException("Unknown physical application path.");
            }

            return directory
                .GetFiles(searchPattern, SearchOption.AllDirectories)
                .Select(fi => new Template(fi, physicalApplicationPath));
        }
    }
}