using System;
using NGineer.Internal;
using System.Collections.Generic;
using System.Linq;
namespace NGineer.BuildGenerators
{
    public class DictionaryGenerator : IGenerator
    {
        private static readonly Type DictionaryType = typeof(IDictionary<,>).GetGenericTypeDefinition();

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            var dictionaryInterface = GetIDictionaryInterface(type);
            if (dictionaryInterface != null)
            {
                var dictionaryType = typeof(Dictionary<, >)
                    .MakeGenericType(KeyType(dictionaryInterface), ValueType(dictionaryInterface));
                return type.IsAssignableFrom(dictionaryType);
            }
            return false;
        }

        public Internal.ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            var dictionaryInterface = GetIDictionaryInterface(type);
            var dictionaryType = typeof(Dictionary<, >)
                .MakeGenericType(KeyType(dictionaryInterface), ValueType(dictionaryInterface));
            var instance = dictionaryType.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            return new ObjectBuildRecord(dictionaryType, instance, false);
        }

        private static Type KeyType(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        private static Type ValueType(Type type)
        {
            return type.GetGenericArguments()[1];
        }

        private static bool IsDictionaryType(Type type)
        {
            return type.IsGenericType && DictionaryType.Equals(type.GetGenericTypeDefinition());
        }

        private static Type GetIDictionaryInterface(Type type)
        {
            if (IsDictionaryType(type))
            {
                return type;
            }
            else
            {
                var ifaces = type.GetInterfaces();
                return ifaces.FirstOrDefault(IsDictionaryType);
            }
        }
    }
}

