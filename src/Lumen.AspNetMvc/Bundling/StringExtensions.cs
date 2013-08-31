using System;
using System.IO;

namespace Lumen.AspNetMvc.Bundling
{
    internal static class StringExtensions
    {
        public static string GetAbsolutePath(this string virtualPath)
        {
            string relativePath = virtualPath.Replace("~/", "").Replace("/", "\\");
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }

        public static string ToCamelCase(this string value)
        {
            Ensure.NotNullOrEmpty(value, "value");

            char ch = value[0];
            return char.ToLower(ch) + value.Substring(1);
        }
    }
}