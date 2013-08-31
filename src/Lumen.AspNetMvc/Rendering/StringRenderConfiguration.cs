namespace Lumen.AspNetMvc.Rendering
{
    /// <summary>
    /// Configuration settings.
    /// </summary>
    public class StringRenderConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="StringRenderConfiguration"/>.
        /// </summary>
        public StringRenderConfiguration()
        {
            ClientValidationEnabled = true;
            IsMobileDevice = false;
            UnobtrusiveJavaScriptEnabled = true;
        }

        /// <summary>
        /// Returns a value indicating whether client validation is enabled. The default value is true.
        /// </summary>
        public bool ClientValidationEnabled { get; set; }

        /// <summary>
        /// Returns a value indicating whether the view should be rendered for a mobile device. The default value is false.
        /// </summary>
        public bool IsMobileDevice { get; set; }

        /// <summary>
        /// Returns a value indicating whether unobtrusive JavaScript is enabled. The default value is true.
        /// </summary>
        public bool UnobtrusiveJavaScriptEnabled { get; set; }
    }
}