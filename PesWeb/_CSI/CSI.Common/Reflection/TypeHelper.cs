using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Common.Reflection
{
    public static class TypeHelper
    {
        public static Type ResolveType(string typeName, Assembly assembly)
        {
            return assembly.GetType(typeName, false);
        }
        public static Type GetTypeFromName(string fullName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName.Equals(fullName));
        }
        public static List<Type> GetAllTypeOf<T>()
        {
            return GetAllTypeOf(typeof(T));
        }
        public static List<Type> GetAllTypeOf(Type type)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t))
                .ToList();
        }
    }
}
