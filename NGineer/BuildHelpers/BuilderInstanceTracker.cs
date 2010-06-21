using System;
using System.Linq;
using NGineer.BuildGenerators;

namespace NGineer.BuildHelpers
{
    public static class BuilderInstanceTracker
    {
        private static readonly IGenerator[] IgnoredGenerators = new IGenerator[]
            {
                new BlittableTypesGenerator(), new NullableTypeGenerator(), new ListGenerator(), new ArrayGenerator(),
                new DateTimeGenerator(), new BoolGenerator(), new EnumGenerator(), new CharGenerator(), new SimpleMemberNameStringGenerator()
            };

        public static bool IncludeInCount(Type type)
        {
            return IgnoredGenerators.FirstOrDefault(g => g.GeneratesType(type, null, null)) == null;
        }

        public static bool ExcludeInCount(Type type)
        {
            return !IncludeInCount(type);
        }
    }
}