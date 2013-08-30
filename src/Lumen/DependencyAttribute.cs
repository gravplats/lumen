using System;

namespace Lumen
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyAttribute : Attribute
    {
        public DependencyAttribute(params Type[] dependencies)
        {
            Dependencies = dependencies;
        }

        public Type[] Dependencies { get; private set; }
    }
}