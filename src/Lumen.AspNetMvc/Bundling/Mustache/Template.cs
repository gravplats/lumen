using System.IO;

namespace Lumen.AspNetMvc.Bundling.Mustache
{
    public class Template
    {
        public Template(FileInfo file, string physicalApplicationPath, string templateTopVirtualPath)
        {
            string vp = templateTopVirtualPath;
            if (templateTopVirtualPath[templateTopVirtualPath.Length - 1] != '/')
            {
                vp = vp + "/";
            }

            VirtualPath = file.FullName.Substring(physicalApplicationPath.Length).Replace('\\', '/').Insert(0, "~/");
            Name = VirtualPath.Substring(vp.Length).Replace('/', '.').Replace(file.Extension, string.Empty);
        }

        public string Name { get; private set; }

        public string VirtualPath { get; private set; }
    }
}