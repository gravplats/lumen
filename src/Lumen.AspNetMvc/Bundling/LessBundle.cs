using System.Web.Optimization;
using dotless.Core;

namespace Lumen.AspNetMvc.Bundling
{
    /// <summary>
    /// Transforms LESS to CSS and minifies the result.
    /// </summary>
    public class LessBundle : Bundle
    {
        /// <summary>
        /// Provides functionality for transforming LESS to CSS.
        /// </summary>
        public class LessTransform : IBundleTransform
        {
            /// <summary>
            /// Transform LESS to CSS.
            /// </summary>
            /// <param name="context">The bundle context.</param>
            /// <param name="response">The bundle response.</param>
            public void Process(BundleContext context, BundleResponse response)
            {
                response.Content = Less.Parse(response.Content);
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="LessBundle" />.
        /// </summary>
        /// <param name="virtualPath"></param>
        public LessBundle(string virtualPath) : base(virtualPath, new LessTransform(), new CssMinify()) { }
    }
}