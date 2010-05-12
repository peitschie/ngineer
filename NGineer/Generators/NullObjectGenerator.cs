
using System;

namespace NGineer.Generators
{
    public class NullObjectGenerator<TType> : SimpleGenerator<TType>
        where TType : class 
    {
        public NullObjectGenerator() : base(0)
        {
        }

        protected override TType Generate()
        {
            return null;
        }
    }
}