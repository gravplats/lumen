using System;

namespace Lumen.AspNetMvc.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AllowAuthenticatedAnonymousAttribute : Attribute { }
}