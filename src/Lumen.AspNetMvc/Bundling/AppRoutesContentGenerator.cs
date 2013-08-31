using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace Lumen.AspNetMvc.Bundling
{
    public class AppRoutesContentGenerator : IDynamicContentGenerator
    {
        private readonly string applicationGlobalVariable;
        private readonly Type routesType;

        public AppRoutesContentGenerator(string applicationGlobalVariable, Type routesType)
        {
            this.applicationGlobalVariable = applicationGlobalVariable;
            this.routesType = routesType;
        }

        public IEnumerable<string> WatchedVirtualPaths
        {
            get { yield break; }
        }

        public string GenerateContent(HttpContextBase httpContext)
        {
            string applicationPath = GetApplicationPath(httpContext);
            using (var writer = new StringWriter())
            {
                writer.WriteLine("!function() {");
                writer.WriteLine("    {0}.routes = {{", applicationGlobalVariable);

                string separator = "," + Environment.NewLine;
                string routes = string.Join(separator, GetRoutes().Select(x => string.Format("        '{0}': '{1}'", GetRouteName(x.Key), GetRouteValue(x.Value))));
                writer.WriteLine(routes);

                writer.WriteLine("    };");
                writer.WriteLine("    {0}.routes.applicationPath = '{1}';", applicationGlobalVariable, applicationPath);
                writer.WriteLine("}()");

                return writer.ToString();
            }
        }

        protected virtual string GetApplicationPath(HttpContextBase httpContext)
        {
            return httpContext.Request.ApplicationPath == "/" ? "" : httpContext.Request.ApplicationPath;
        }

        private Dictionary<string, string> GetRoutes()
        {
            var values = new Dictionary<string, string>();

            var fields = routesType.GetFields(BindingFlags.Public | BindingFlags.Static).Where(fi => fi.IsLiteral);
            foreach (var field in fields)
            {
                string value = field.GetValue(null) as string;
                if (value == null)
                {
                    throw new InvalidOperationException("Missing value. Is this even possible?");
                }

                values.Add(field.Name, value);
            }

            return values;
        }

        private static string GetRouteName(string name)
        {
            return name.ToCamelCase();
        }

        private static string GetRouteValue(string value)
        {
            var matches = Regex.Matches(value, "{[^}]+}");
            foreach (Match match in matches)
            {
                string token = match.Value;
                string name = token.Substring(0, token.Length);

                value = value.Replace(token, name.Substring(0, name.IndexOf(":")) + "}");
            }

            return value;
        }
    }
}