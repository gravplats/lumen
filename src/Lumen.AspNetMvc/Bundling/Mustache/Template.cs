using System.IO;

namespace Lumen.AspNetMvc.Bundling.Mustache
{
    public class Template
    {
        public Template(FileInfo file, string physicalApplicationPath)
        {
            VirtualPath = file.FullName.Substring(physicalApplicationPath.Length).Replace('\\', '/').Insert(0, "~/");
            Name = file.Name.Replace(file.Extension, string.Empty);
        }

        public string Name { get; private set; }

        public string VirtualPath { get; private set; }
    }
}