using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lumen
{
    public static class EnumerableExtensions
    {
        // http://stackoverflow.com/questions/4106862/how-to-sort-depended-objects-by-dependency
        class TopologicalSort<T>
        {
            private readonly IEnumerable<T> source;
            private readonly Func<T, IEnumerable<T>> dependencies;

            public TopologicalSort(IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies)
            {
                this.source = Ensure.NotNull(source, "source");
                this.dependencies = Ensure.NotNull(dependencies, "dependencies");
            }

            public IEnumerable<T> Sort()
            {
                var sorted = new List<T>();
                var visited = new HashSet<T>();

                foreach (var item in source)
                {
                    Visit(item, visited, sorted);
                }

                return sorted;
            }

            private void Visit(T item, ISet<T> visited, ICollection<T> sorted)
            {
                if (visited.Contains(item))
                {
                    return;
                }

                visited.Add(item);

                foreach (var dependency in dependencies(item))
                {
                    Visit(dependency, visited, sorted);
                }

                sorted.Add(item);
            }
        }

        public static IEnumerable<Type> TopoSort(this IEnumerable<Type> source)
        {
            var map = new Dictionary<Type, List<Type>>();
            foreach (var type in source)
            {
                var dependencies = new List<Type>();
                if (Attribute.IsDefined(type, typeof(DependencyAttribute)))
                {
                    var attribute = (DependencyAttribute)type.GetCustomAttribute(typeof(DependencyAttribute), false);
                    dependencies.AddRange(attribute.Dependencies);
                }

                map.Add(type, dependencies);
            }

            return new TopologicalSort<Type>(source, type => map[type]).Sort();
        }
    }
}