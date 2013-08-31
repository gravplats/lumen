using System.Web.Optimization;

namespace Lumen.AspNetMvc.Bundling
{
    public static class BundleExtensions
    {
        public static Bundle AddTo(this Bundle bundle, BundleCollection collection)
        {
            collection.Add(bundle);
            return bundle;
        }
    }
}