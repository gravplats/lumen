using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace Lumen.AspNetMvc.Bundling
{
    /// <summary>
    /// Provides functionality for adding dynamically generated content to a bundle.
    /// </summary>
    public class DynamicContentBundle : ScriptBundle
    {
        // Determine if we're running in debug mode.
        private readonly bool debug;
        // Provides a map between a virtual path and dynamic content.
        private readonly Dictionary<string, IDynamicContentGenerator> dynamicContentRegistry = new Dictionary<string, IDynamicContentGenerator>();

        /// <summary>
        /// Creates an instance of <see cref="DynamicContentBundle"/>.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the bundle.</param>
        public DynamicContentBundle(string virtualPath)
            : base(virtualPath)
        {
            debug = HttpContext.Current.IsDebuggingEnabled;
        }

        /// <summary>
        /// Renames the file giving us a chance to ignore dynamic JavaScript files in the version control system. The default
        /// extensions of dynamically generated JavaScript files is .dynamic.js.
        /// </summary>
        public Func<string, string> Rename = virtualPath => virtualPath.Replace(".js", ".dynamic.js");

        /// <inheritdoc />
        public new DynamicContentBundle Include(params string[] virtualPath)
        {
            return base.Include(virtualPath) as DynamicContentBundle;
        }

        /// <inheritdoc />
        public new DynamicContentBundle IncludeDirectory(string directoryVirtualPath, string searchPattern, bool searchSubdirectories = false)
        {
            return base.IncludeDirectory(directoryVirtualPath, searchPattern, searchSubdirectories) as DynamicContentBundle;
        }

        /// <summary>
        /// Include dynamically generated content.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the dynamically generated content.</param>
        /// <param name="content">The dynamically generated content.</param>
        /// <returns>The bundle.</returns>
        public DynamicContentBundle IncludeGeneratedScript(string virtualPath, IDynamicContentGenerator content)
        {
            Ensure.NotNullOrEmpty(virtualPath, "virtualPath");
            Ensure.NotNull(content, "content");

            virtualPath = Rename(virtualPath);

            string absolutePath = virtualPath.GetAbsolutePath();
            // Must generate file before we include it; the file must exist otherwise the web optimization framework won't include it.
            File.WriteAllText(absolutePath, "");

            dynamicContentRegistry.Add(absolutePath, content);

            return Include(virtualPath);
        }

        /// <inheritdoc />
        public override IEnumerable<BundleFile> EnumerateFiles(BundleContext bundleContext)
        {
            foreach (var item in dynamicContentRegistry)
            {
                string absoluteScriptPath = item.Key;
                var dynamicContent = item.Value;

                if (debug)
                {
                    Watch(bundleContext, dynamicContent, absoluteScriptPath);
                }

                File.WriteAllText(absoluteScriptPath, dynamicContent.GenerateContent(bundleContext.HttpContext));
            }

            return base.EnumerateFiles(bundleContext);
        }

        /// <summary>
        /// Watches the specified path and generates new content whenever a files changes.
        /// </summary>
        /// <param name="bundleContext">The bundle context.</param>
        /// <param name="dynamicContent">The dynamic content.</param>
        /// <param name="absoluteScriptPath">The absolute path to the dynamically generated content.</param>
        protected virtual void Watch(BundleContext bundleContext, IDynamicContentGenerator dynamicContent, string absoluteScriptPath)
        {
            foreach (var watchedVirtualPath in dynamicContent.WatchedVirtualPaths)
            {
                string watchedAbsolutePath = watchedVirtualPath.GetAbsolutePath();
                var watcher = new FileSystemWatcher(watchedAbsolutePath)
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true
                };

                watcher.Changed += (sender, e) =>
                {
                    try
                    {
                        File.WriteAllText(absoluteScriptPath, dynamicContent.GenerateContent(bundleContext.HttpContext));
                    }
                    catch (Exception)
                    {
                        // Any exceptions here are most likely due to access permissions.
                        // Suppress or the entire web server will crash.
                    }
                };
            }
        }
    }
}