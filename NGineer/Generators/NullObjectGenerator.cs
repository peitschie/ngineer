
using System;

namespace NGineer.Generators
{
    public class NullObjectGenerator<TType> : SimpleGenerator<TType>
        where TType : class 
    {
        public NullObjectGenerator(int seed) : base(seed)
        {
        }

        protected override TType Generate()
        {
            return null;
        }
    }
}