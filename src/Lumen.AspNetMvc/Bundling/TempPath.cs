using System;
using System.IO;

namespace Lumen.AspNetMvc.Bundling
{
    /// <summary>
    /// Converts a virtual path to an absolute path using the temp path.
    /// </summary>
    /// <remarks>AppHarbor only allows write access to the temp path.</remarks>
    public class TempPath : IDynamicContentPath
    {
        /// <inheritdoc />
        public string GetAbsolutePath(string virtualPath)
        {
            string filename = Path.GetFileName(virtualPath);
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new InvalidOperationException("Virtual path must include filename.");
            }

            string tempPath = Path.GetTempPath();
            return Path.Combine(tempPath, filename);
        }
    }
}