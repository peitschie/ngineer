using NGineer.BuildGenerators;

namespace NGineer
{
    public static class Generators
    {
        public static IGenerator NullGenerator<TType>() where TType : class
        {
            return new NullObjectGenerator<TType>();
        }
    }
}