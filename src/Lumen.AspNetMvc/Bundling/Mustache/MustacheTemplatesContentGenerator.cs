using System.Collections.Generic;
using System.Web;

namespace Lumen.AspNetMvc.Bundling.Mustache
{
    public class MustacheTemplatesContentGenerator : IDynamicContentGenerator
    {
        private readonly string templateTopVirtualPath;

        private readonly TemplateFinder finder;
        private readonly JavaScriptGenerator generator;

        public MustacheTemplatesContentGenerator(string globalApplicationVariable, string templateTopVirtualPath, string searchPattern = "*.cshtml")
        {
            this.templateTopVirtualPath = templateTopVirtualPath;

            finder = new TemplateFinder(templateTopVirtualPath, searchPattern);
            generator = new JavaScriptGenerator(globalApplicationVariable);
        }

        public IEnumerable<string> WatchedVirtualPaths
        {
            get { yield return templateTopVirtualPath; }
        }

        public string GenerateContent(HttpContextBase httpContext)
        {
            var templates = finder.GetVirtualPaths(httpContext);
            return generator.CreateContent(templates);
        }
    }
}