using System.Collections.Generic;
using System.Web;

namespace Lumen.AspNetMvc.Bundling.Mustache
{
    public class MustacheTemplates : IDynamicContentGenerator
    {
        private readonly string templateTopVirtualPath;
        private readonly string searchPattern;

        private readonly TemplateFinder finder;
        private readonly JavaScriptGenerator generator;

        public MustacheTemplates(string globalApplicationVariable, string templateTopVirtualPath, string searchPattern = "*.cshtml")
        {
            this.templateTopVirtualPath = templateTopVirtualPath;
            this.searchPattern = searchPattern;

            finder = new TemplateFinder(templateTopVirtualPath, searchPattern);
            generator = new JavaScriptGenerator(globalApplicationVariable);
        }

        public string Filter
        {
            get { return searchPattern; }
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