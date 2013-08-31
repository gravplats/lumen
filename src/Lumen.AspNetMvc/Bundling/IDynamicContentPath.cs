namespace Lumen.AspNetMvc.Bundling
{
    /// <summary>
    /// Provides an interface for converting a virtual path to an absolute path.
    /// </summary>
    public interface IDynamicContentPath
    {
        /// <summary>
        /// Gets the absolute path for the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the JavaScript file.</param>
        /// <returns>The absolute path of the JavaScript file.</returns>
        string GetAbsolutePath(string virtualPath);
    }
}