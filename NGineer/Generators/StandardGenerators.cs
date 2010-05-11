namespace NGineer.Generators
{
    public static class StandardGenerators
    {
        public static IGenerator NullGenerator<TType>(int seed) where TType : class
        {
            return new NullObjectGenerator<TType>(seed);
        }
    }
}