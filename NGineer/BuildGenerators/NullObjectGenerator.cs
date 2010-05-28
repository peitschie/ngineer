using System;

namespace NGineer.BuildGenerators
{
    public class NullObjectGenerator<TType> : SimpleGenerator<TType>
        where TType : class 
    {
        protected override TType Generate(Random random)
        {
            return null;
        }
    }
}