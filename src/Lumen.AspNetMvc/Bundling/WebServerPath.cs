namespace Lumen.AspNetMvc.Bundling
{
    /// <summary>
    /// Converts the virtual path to an absolute path relative the current web server.
    /// </summary>
    public class WebServerPath : IDynamicContentPath
    {
        /// <inheritdoc />
        public string GetAbsolutePath(string virtualPath)
        {
            return virtualPath.GetAbsolutePath();
        }
    }
}