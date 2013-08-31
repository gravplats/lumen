using System.Collections.Generic;
using System.IO;
using Jurassic;
using Lumen.AspNetMvc.Rendering;

namespace Lumen.AspNetMvc.Bundling.Mustache
{
    public class JavaScriptGenerator
    {
        private readonly string globalApplicationVariable;

        public JavaScriptGenerator(string globalApplicationVariable)
        {
            this.globalApplicationVariable = Ensure.NotNullOrEmpty(globalApplicationVariable, "globalApplicationVariable");
        }

        public string CreateContent(IEnumerable<Template> templates)
        {
            const string localHoganTemplateVariable = "HT";
            const string localTemplatesVariable = "templates";

            using (var writer = new StringWriter())
            {
                var scriptEngine = CreateScriptEngine();

                writer.WriteLine("!function({0}) {{", localHoganTemplateVariable);
                writer.WriteLine("    var {0} = {1}.templates = {{}};", localTemplatesVariable, globalApplicationVariable);

                foreach (var template in templates)
                {
                    string html = ViewWriter.ToString(template.VirtualPath);
                    string compiledMustacheTemplate = scriptEngine.CallGlobalFunction<string>("compile", html);

                    writer.WriteLine("    {0}['{1}'] = new {2}({3});", localTemplatesVariable, template.Name, localHoganTemplateVariable, compiledMustacheTemplate);
                }

                writer.WriteLine("}(Hogan.Template)");

                return writer.ToString();
            }
        }

        private static ScriptEngine CreateScriptEngine()
        {
            var engine = new ScriptEngine();
            engine.Execute(Resources.HoganJs);

            using (var writer = new StringWriter())
            {
                writer.WriteLine("var compile = function(template) {");
                writer.WriteLine("    return Hogan.compile(template, { asString: true });");
                writer.WriteLine("};");

                string script = writer.ToString();
                engine.Execute(script);
            }

            return engine;
        }
    }
}