using System;
using NGineer.Internal;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    public class EnumGenerator : IGenerator
    {
        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return type.IsEnum;
        }

        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            return new ObjectBuildRecord(type, GetRandomEnum(type, session.Random), false);
        }

        private static object GetRandomEnum(Type type, Random random)
        {
            var array = EnumUtils.GetValues(type);
            var index = random.Next(array.Length);
            return array.GetValue(index);
        }
    }
}