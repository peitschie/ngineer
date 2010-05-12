namespace NGineer.Generators
{
    public static class StandardGenerators
    {
        public static IGenerator NullGenerator<TType>() where TType : class
        {
            return new NullObjectGenerator<TType>();
        }
    }
}