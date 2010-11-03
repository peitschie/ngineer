using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NGineer.Utils
{
    public static class ListUtils
    {
        private static readonly Type EnumerableType = typeof(IEnumerable<>).GetGenericTypeDefinition();

        private static bool IsEnumerableType(Type type)
        {
            return type.IsGenericType && EnumerableType.Equals(type.GetGenericTypeDefinition());
        }

        public static Type GetEnumerableElementType(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            else if (IsEnumerableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            else
            {
                var ifaces = type.GetInterfaces();
                var enumerableType = ifaces.FirstOrDefault(IsEnumerableType);
                return enumerableType != null ? enumerableType.GetGenericArguments()[0] : null;
            }
        }

        public static bool IsGenericEnumerable(this Type type)
        {
            return GetEnumerableElementType(type) != null;
        }
    }
}